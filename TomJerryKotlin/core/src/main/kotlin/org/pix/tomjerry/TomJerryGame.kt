package org.pix.tomjerry

import com.badlogic.gdx.ApplicationAdapter
import com.badlogic.gdx.Gdx
import com.badlogic.gdx.Input
import com.badlogic.gdx.InputAdapter
import com.badlogic.gdx.graphics.GL20
import com.badlogic.gdx.graphics.PerspectiveCamera
import com.badlogic.gdx.graphics.VertexAttributes
import com.badlogic.gdx.graphics.g3d.Environment
import com.badlogic.gdx.graphics.g3d.Material
import com.badlogic.gdx.graphics.g3d.Model
import com.badlogic.gdx.graphics.g3d.ModelBatch
import com.badlogic.gdx.graphics.g3d.ModelInstance
import com.badlogic.gdx.graphics.g3d.attributes.ColorAttribute
import com.badlogic.gdx.graphics.g3d.environment.DirectionalLight
import com.badlogic.gdx.graphics.g3d.utils.ModelBuilder
import com.badlogic.gdx.graphics.g2d.BitmapFont
import com.badlogic.gdx.graphics.g2d.SpriteBatch
import com.badlogic.gdx.graphics.glutils.ShapeRenderer
import com.badlogic.gdx.graphics.Color
import com.badlogic.gdx.math.MathUtils
import com.badlogic.gdx.math.Vector3

/** Kotlin/LibGDX first-person Tom & Jerry game. */
class TomJerryGame : ApplicationAdapter() {
    private data class Pickup(val label: String, val instance: ModelInstance, var active: Boolean = true)
    private lateinit var camera: PerspectiveCamera
    private lateinit var modelBatch: ModelBatch
    private lateinit var environment: Environment
    private lateinit var shape: ShapeRenderer
    private lateinit var hudBatch: SpriteBatch
    private lateinit var font: BitmapFont
    private val models = mutableListOf<Model>()
    private val world = mutableListOf<ModelInstance>()
    private val cheese = mutableListOf<Vector3>()
    private val cheeseInstances = mutableListOf<ModelInstance>()
    private val trapParts = mutableListOf<ModelInstance>()
    private val pickups = mutableListOf<Pickup>()
    private val breakables = mutableListOf<ModelInstance>()
    private lateinit var tom: ModelInstance
    private var owner: ModelInstance? = null
    private val player = Vector3(0f, 1.65f, 6f)
    private val tomPosition = Vector3(0f, 0.65f, -5f)
    private val ownerPosition = Vector3(0f, 0.7f, 8.4f)
    private var yaw = 180f
    private var pitch = 0f
    private var cheeseCount = 0
    private var trapped = 0f
    private var gameOver = false
    private var won = false
    private var leftPointer = -1
    private var rightPointer = -1
    private var leftStartX = 0f
    private var leftStartY = 0f
    private var moveX = 0f
    private var moveY = 0f
    private var lastRightX = 0f
    private var lastRightY = 0f
    private var trapPlaced = false
    private var trapBaited = false
    private var trapPosition = Vector3()
    private var held: Pickup? = null
    private var destroyedCount = 0
    private var ownerActive = false
    private var tomExpelled = false
    private var fridgeOpen = false
    private var fridgeDoor: ModelInstance? = null
    private var message = "Find the cheese and stay away from Tom!"
    private var messageTimer = 5f
    private val forward = Vector3()
    private val right = Vector3()
    private val tmp = Vector3()

    override fun create() {
        camera = PerspectiveCamera(68f, Gdx.graphics.width.toFloat(), Gdx.graphics.height.toFloat()).apply {
            near = 0.1f
            far = 100f
        }
        modelBatch = ModelBatch()
        environment = Environment().apply {
            set(ColorAttribute(ColorAttribute.AmbientLight, 0.78f, 0.78f, 0.82f, 1f))
            add(DirectionalLight().set(0.95f, 0.9f, 0.82f, -1f, -2f, -1f))
        }
        shape = ShapeRenderer()
        hudBatch = SpriteBatch()
        font = BitmapFont()
        buildWorld()
        Gdx.input.inputProcessor = object : InputAdapter() {
            override fun touchDown(screenX: Int, screenY: Int, pointer: Int, button: Int): Boolean {
                if (gameOver || won) { resetGame(); return true }
                val w = Gdx.graphics.width.toFloat()
                val h = Gdx.graphics.height.toFloat()
                if (screenY > h * 0.68f && screenX > w * 0.80f) {
                    placeTrap()
                } else if (screenY > h * 0.68f && screenX > w * 0.48f) {
                    useAction()
                } else if (screenX < w * 0.45f && leftPointer == -1) {
                    leftPointer = pointer
                    leftStartX = screenX.toFloat()
                    leftStartY = screenY.toFloat()
                    moveX = 0f
                    moveY = 0f
                } else if (rightPointer == -1) {
                    rightPointer = pointer
                    lastRightX = screenX.toFloat()
                    lastRightY = screenY.toFloat()
                }
                return true
            }
            override fun touchDragged(screenX: Int, screenY: Int, pointer: Int): Boolean {
                if (pointer == leftPointer) {
                    moveX = ((screenX - leftStartX) / 85f).coerceIn(-1f, 1f)
                    moveY = ((screenY - leftStartY) / 85f).coerceIn(-1f, 1f)
                } else if (pointer == rightPointer) {
                    yaw -= (screenX - lastRightX) * 0.22f
                    pitch = (pitch + (screenY - lastRightY) * 0.16f).coerceIn(-35f, 35f)
                    lastRightX = screenX.toFloat()
                    lastRightY = screenY.toFloat()
                }
                return true
            }
            override fun touchUp(screenX: Int, screenY: Int, pointer: Int, button: Int): Boolean {
                if (pointer == leftPointer) { leftPointer = -1; moveX = 0f; moveY = 0f }
                if (pointer == rightPointer) rightPointer = -1
                return true
            }
        }
        updateCamera()
    }

    private fun makeBox(x: Float, y: Float, z: Float, sx: Float, sy: Float, sz: Float, color: Color): ModelInstance {
        val model = ModelBuilder().createBox(sx, sy, sz,
            Material(ColorAttribute.createDiffuse(color)),
            (VertexAttributes.Usage.Position or VertexAttributes.Usage.Normal).toLong())
        models.add(model)
        return ModelInstance(model).also { it.transform.setToTranslation(x, y, z); world.add(it) }
    }

    private fun makeSphere(x: Float, y: Float, z: Float, size: Float, color: Color): ModelInstance {
        val model = ModelBuilder().createSphere(size, size, size, 18, 14,
            Material(ColorAttribute.createDiffuse(color)),
            (VertexAttributes.Usage.Position or VertexAttributes.Usage.Normal).toLong())
        models.add(model)
        return ModelInstance(model).also { it.transform.setToTranslation(x, y, z); world.add(it) }
    }

    private fun buildWorld() {
        makeBox(0f, -0.15f, 0f, 20f, 0.3f, 20f, Color.valueOf("b88b62"))
        makeBox(-5.5f, 2f, 10f, 9f, 4f, 0.25f, Color.valueOf("e7d5b7"))
        makeBox(7.5f, 2f, 10f, 5f, 4f, 0.25f, Color.valueOf("e7d5b7"))
        makeBox(0f, 2f, -10f, 20f, 4f, 0.25f, Color.valueOf("e7d5b7"))
        makeBox(-10f, 2f, 0f, 0.25f, 4f, 20f, Color.valueOf("e7d5b7"))
        makeBox(10f, 2f, 0f, 0.25f, 4f, 20f, Color.valueOf("e7d5b7"))
        // Kitchen counters and refrigerator.
        makeBox(-3.2f, 1f, -2f, 2.2f, 2f, 0.8f, Color.valueOf("8c573d"))
        makeBox(-5.4f, 1.25f, -5.5f, 1.5f, 2.5f, 1.5f, Color.valueOf("d9e3e8"))
        fridgeDoor = makeBox(-5.4f, 1.25f, -4.72f, 1.42f, 2.45f, 0.08f, Color.valueOf("b9cbd3"))
        makeBox(3.8f, 0.8f, -3.5f, 2.8f, 1.6f, 1f, Color.valueOf("9a6545"))
        // Dining table and breakable chairs.
        makeBox(3f, 1.35f, 3f, 3.4f, 0.18f, 2f, Color.valueOf("70452f"))
        breakables.add(makeBox(1.7f, 0.65f, 3f, 0.55f, 1.3f, 0.55f, Color.valueOf("70452f")))
        breakables.add(makeBox(4.3f, 0.65f, 3f, 0.55f, 1.3f, 0.55f, Color.valueOf("70452f")))
        breakables.add(makeSphere(-1.8f, 0.55f, -3.2f, 0.85f, Color.valueOf("d68bca")))
        // Tom and his ears.
        tom = makeSphere(tomPosition.x, tomPosition.y, tomPosition.z, 1.15f, Color.valueOf("777b83"))
        makeSphere(tomPosition.x - 0.43f, 1.12f, tomPosition.z, 0.42f, Color.valueOf("777b83"))
        makeSphere(tomPosition.x + 0.43f, 1.12f, tomPosition.z, 0.42f, Color.valueOf("777b83"))
        // Cheese objectives.
        val spots = listOf(Vector3(-2f, 0.35f, 2f), Vector3(5f, 0.35f, -1f), Vector3(-6f, 0.35f, 5f))
        cheese.addAll(spots)
        spots.forEach { cheeseInstances.add(makeSphere(it.x, it.y, it.z, 0.48f, Color.valueOf("ffd447"))) }
        // Food props near the refrigerator can be carried and used as trap bait.
        pickups.add(Pickup("Apple", makeSphere(-4.2f, 0.45f, -3.5f, 0.55f, Color.valueOf("df4b3f"))))
        pickups.add(Pickup("Bread", makeBox(-6.8f, 0.45f, -3.5f, 0.8f, 0.45f, 0.55f, Color.valueOf("d5a65a"))))
        pickups.add(Pickup("Cheese", makeBox(-4.2f, 0.4f, -6.8f, 0.65f, 0.35f, 0.55f, Color.valueOf("ffd447"))))
        makeBox(0f, 2.5f, -9.82f, 3.2f, 1.4f, 0.04f, Color.valueOf("8dc9e8"))
    }

    private fun updateCamera() {
        val yawRad = yaw * MathUtils.degreesToRadians
        val pitchRad = pitch * MathUtils.degreesToRadians
        forward.set(-MathUtils.sin(yawRad) * MathUtils.cos(pitchRad), MathUtils.sin(pitchRad),
            -MathUtils.cos(yawRad) * MathUtils.cos(pitchRad)).nor()
        camera.position.set(player)
        camera.direction.set(forward)
        camera.up.set(Vector3.Y)
        camera.update()
    }

    private fun setMessage(text: String, seconds: Float = 3.5f) {
        message = text
        messageTimer = seconds
    }

    private fun useAction() {
        if (held != null) {
            val item = held!!
            if (trapPlaced && !trapBaited && Vector3(player.x, 0f, player.z).dst(Vector3(trapPosition.x, 0f, trapPosition.z)) < 4.5f) {
                item.active = false
                world.remove(item.instance)
                held = null
                trapBaited = true
                setMessage("${item.label} placed on the mousetrap! Tom will investigate.")
            } else {
                val drop = Vector3(player.x + forward.x * 2f, 0.45f, player.z + forward.z * 2f)
                item.instance.transform.setToTranslation(drop)
                if (!world.contains(item.instance)) world.add(item.instance)
                item.active = true
                held = null
                setMessage("You dropped the ${item.label}.")
            }
            return
        }
        val nearest = pickups.filter { it.active }.minByOrNull { it.instance.transform.getTranslation(Vector3()).dst(player) }
        if (nearest != null && nearest.instance.transform.getTranslation(Vector3()).dst(player) < 3.2f) {
            held = nearest
            nearest.active = false
            world.remove(nearest.instance)
            setMessage("${nearest.label} picked up. Go to the trap and tap USE to bait it.")
            return
        }
        if (player.dst(Vector3(-5.4f, 0f, -5.5f)) < 3.5f) {
            fridgeOpen = !fridgeOpen
            fridgeDoor?.transform?.setToTranslation(if (fridgeOpen) -6.35f else -5.4f, 1.25f, -4.72f)
            setMessage(if (fridgeOpen) "Fridge opened. Food is ready to grab!" else "Fridge closed.")
        } else {
            setMessage("Move close to food or the refrigerator, then tap USE.")
        }
    }

    private fun placeTrap() {
        if (trapPlaced) {
            setMessage(if (trapBaited) "The trap is baited and ready!" else "Pick up food and tap USE near the trap.")
            return
        }
        if (cheeseCount <= 0) {
            setMessage("Collect at least one cheese before setting the trap.")
            return
        }
        trapPlaced = true
        cheeseCount--
        trapPosition.set(player.x + forward.x * 1.6f, 0.12f, player.z + forward.z * 1.6f)
        trapParts.clear()
        trapParts.add(makeBox(trapPosition.x, trapPosition.y, trapPosition.z, 0.9f, 0.18f, 0.65f, Color.LIGHT_GRAY))
        trapParts.add(makeBox(trapPosition.x, trapPosition.y + 0.18f, trapPosition.z, 0.62f, 0.08f, 0.08f, Color.DARK_GRAY))
        setMessage("Mousetrap set. Pick up food and use it as bait!")
    }

    private fun updateGame(delta: Float) {
        val dt = delta.coerceAtMost(0.05f)
        if (messageTimer > 0f) messageTimer -= dt
        if (gameOver || won) return
        val yawRad = yaw * MathUtils.degreesToRadians
        forward.set(-MathUtils.sin(yawRad), 0f, -MathUtils.cos(yawRad)).nor()
        right.set(forward.z, 0f, -forward.x).nor()
        val move = tmp.setZero().mulAdd(forward, -moveY).mulAdd(right, moveX)
        if (Gdx.input.isKeyPressed(Input.Keys.W)) move.add(forward)
        if (Gdx.input.isKeyPressed(Input.Keys.S)) move.sub(forward)
        if (Gdx.input.isKeyPressed(Input.Keys.A)) move.sub(right)
        if (Gdx.input.isKeyPressed(Input.Keys.D)) move.add(right)
        if (!move.isZero) player.mulAdd(move.nor(), 4f * dt)
        player.x = player.x.coerceIn(-8.7f, 8.7f)
        player.z = player.z.coerceIn(-8.7f, 8.7f)

        if (!tomExpelled) {
            if (trapped > 0f) {
                trapped -= dt
            } else if (!ownerActive) {
                val chase = Vector3(player.x - tomPosition.x, 0f, player.z - tomPosition.z)
                if (chase.len2() > 0.01f) tomPosition.mulAdd(chase.nor(), 1.25f * dt)
                tom.transform.setToTranslation(tomPosition)
            }
            // Tom smashes furniture when he runs into it.
            for (piece in breakables.toList()) {
                if (piece.transform.getTranslation(Vector3()).dst(tomPosition) < 1.25f && world.remove(piece)) {
                    destroyedCount++
                    setMessage("CRASH! Tom smashed something! ($destroyedCount/3)")
                    if (destroyedCount >= 3) callHomeowner()
                }
            }
            if (trapPlaced && trapBaited && tomPosition.dst(trapPosition) < 1.5f) {
                trapped = 7f
                trapBaited = false
                setMessage("SNAP! Tom got caught in the mousetrap!")
            }
            if (trapPlaced && !trapBaited && tomPosition.dst(player) < 2.0f && trapped <= 0f) {
                trapped = 3f
                setMessage("Tom is distracted by the trap!")
            }
            if (tomPosition.dst(player) < 0.85f && trapped <= 0f) gameOver = true
        } else if (ownerActive) {
            val chaseOwner = Vector3(tomPosition.x - ownerPosition.x, 0f, tomPosition.z - ownerPosition.z)
            if (chaseOwner.len2() > 0.05f) ownerPosition.mulAdd(chaseOwner.nor(), 4.5f * dt)
            owner?.transform?.setToTranslation(ownerPosition)
            if (ownerPosition.dst(tomPosition) < 1.6f) {
                tomPosition.z += 7f * dt
                tom.transform.setToTranslation(tomPosition)
                if (tomPosition.z > 12f) {
                    tomExpelled = true
                    ownerActive = false
                    owner?.let { world.remove(it) }
                    setMessage("JERRY: Hehehehe! Tom got kicked out!", 8f)
                }
            }
        }

        for (i in cheese.indices.reversed()) {
            if (player.dst(cheese[i]) < 1.0f) {
                cheese.removeAt(i)
                world.remove(cheeseInstances[i])
                cheeseCount++
                setMessage("Cheese collected!")
            }
        }
        if (cheeseCount >= 3) {
            won = true
            setMessage("Jerry wins! All the cheese collected.")
        }
        // Keep a carried object visibly in front of the first-person camera.
        held?.instance?.transform?.setToTranslation(
            player.x + forward.x * 0.62f + right.x * 0.32f,
            player.y - 0.42f,
            player.z + forward.z * 0.62f + right.z * 0.32f
        )
        updateCamera()
    }

    private fun callHomeowner() {
        if (ownerActive) return
        ownerActive = true
        ownerPosition.set(0f, 0.7f, 8.4f)
        owner = makeSphere(ownerPosition.x, ownerPosition.y, ownerPosition.z, 1.25f, Color.valueOf("e6b58a"))
        makeSphere(ownerPosition.x, 1.55f, ownerPosition.z, 0.85f, Color.valueOf("3c302c"))
        setMessage("The homeowner is furious! He's chasing Tom out of the house!", 6f)
    }

    override fun render() {
        updateGame(Gdx.graphics.deltaTime)
        Gdx.gl.glViewport(0, 0, Gdx.graphics.width, Gdx.graphics.height)
        Gdx.gl.glClearColor(0.48f, 0.68f, 0.82f, 1f)
        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT or GL20.GL_DEPTH_BUFFER_BIT)
        Gdx.gl.glEnable(GL20.GL_DEPTH_TEST)
        modelBatch.begin(camera)
        world.forEach { modelBatch.render(it, environment) }
        modelBatch.end()
        drawHud()
    }

    private fun drawHud() {
        val w = Gdx.graphics.width.toFloat()
        val h = Gdx.graphics.height.toFloat()
        shape.projectionMatrix.setToOrtho2D(0f, 0f, w, h)
        shape.begin(ShapeRenderer.ShapeType.Filled)
        shape.color = Color(0f, 0f, 0f, 0.28f)
        shape.circle(92f, 92f, 54f)
        shape.circle(w - 95f, 95f, 54f)
        shape.circle(w * 0.64f, 95f, 48f)
        shape.color = Color(1f, 1f, 1f, 0.35f)
        shape.circle(92f + moveX * 28f, 92f - moveY * 28f, 19f)
        shape.end()
        hudBatch.begin()
        font.color = Color.WHITE
        font.data.setScale(1.5f)
        font.draw(hudBatch, "CHEESE: $cheeseCount / 3", 22f, h - 24f)
        font.data.setScale(1.0f)
        font.draw(hudBatch, "MOVE", 62f, 30f)
        font.draw(hudBatch, "USE", w * 0.64f - 16f, 30f)
        font.draw(hudBatch, "TRAP", w - 125f, 30f)
        font.draw(hudBatch, "BROKEN: $destroyedCount / 3", 22f, h - 50f)
        if (held != null) font.draw(hudBatch, "HOLDING: ${held!!.label}", 22f, h - 76f)
        if (trapPlaced) font.draw(hudBatch, if (trapBaited) "TRAP: BAITED" else "TRAP: WAITING FOR FOOD", 22f, h - 102f)
        if (messageTimer > 0f) font.draw(hudBatch, message, w * 0.28f, h - 28f)
        else font.draw(hudBatch, "LEFT: MOVE   RIGHT: LOOK", w * 0.35f, 30f)
        if (won) font.draw(hudBatch, "JERRY WINS! Tap to restart", w / 2f - 110f, h / 2f)
        if (gameOver) font.draw(hudBatch, "TOM CAUGHT YOU! Tap to restart", w / 2f - 145f, h / 2f)
        if (tomExpelled) font.draw(hudBatch, "JERRY: HEHEHEHE!", w / 2f - 85f, h / 2f + 45f)
        hudBatch.end()
    }

    private fun resetGame() {
        gameOver = false
        won = false
        cheeseCount = 0
        trapped = 0f
        trapPlaced = false
        trapBaited = false
        destroyedCount = 0
        ownerActive = false
        tomExpelled = false
        fridgeOpen = false
        held = null
        player.set(0f, 1.65f, 6f)
        tomPosition.set(0f, 0.65f, -5f)
        tom.transform.setToTranslation(tomPosition)
        owner?.let { world.remove(it) }
        owner = null
        breakables.forEach { if (!world.contains(it)) world.add(it) }
        cheese.clear()
        cheese.addAll(listOf(Vector3(-2f, 0.35f, 2f), Vector3(5f, 0.35f, -1f), Vector3(-6f, 0.35f, 5f)))
        cheeseInstances.forEachIndexed { index, instance ->
            if (!world.contains(instance)) world.add(instance)
            instance.transform.setToTranslation(cheese[index])
        }
        pickups.forEach { item ->
            item.active = true
            if (!world.contains(item.instance)) world.add(item.instance)
        }
        trapParts.forEach { world.remove(it) }
        trapParts.clear()
        fridgeDoor?.transform?.setToTranslation(-5.4f, 1.25f, -4.72f)
        leftPointer = -1
        rightPointer = -1
        moveX = 0f
        moveY = 0f
        yaw = 180f
        pitch = 0f
        setMessage("Find cheese, pick up food, bait the trap, and avoid Tom!", 6f)
        updateCamera()
    }

    override fun resize(width: Int, height: Int) {
        camera.viewportWidth = width.toFloat()
        camera.viewportHeight = height.toFloat()
        camera.update()
    }

    override fun dispose() {
        modelBatch.dispose()
        shape.dispose()
        hudBatch.dispose()
        font.dispose()
        models.forEach { it.dispose() }
    }
}
