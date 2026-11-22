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
import kotlin.math.max
import kotlin.math.min

/** First-person mobile Tom & Jerry prototype. All gameplay code is Kotlin. */
class TomJerryGame : ApplicationAdapter() {
    private lateinit var camera: PerspectiveCamera
    private lateinit var modelBatch: ModelBatch
    private lateinit var environment: Environment
    private lateinit var shape: ShapeRenderer
    private lateinit var hudBatch: SpriteBatch
    private lateinit var font: BitmapFont
    private val models = mutableListOf<Model>()
    private val world = mutableListOf<ModelInstance>()
    private val cheese = mutableListOf<Vector3>()
    private lateinit var tom: ModelInstance
    private val player = Vector3(0f, 1.65f, 6f)
    private val tomPosition = Vector3(0f, 0.65f, -5f)
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
    private val forward = Vector3()
    private val right = Vector3()
    private val tmp = Vector3()

    override fun create() {
        camera = PerspectiveCamera(68f, Gdx.graphics.width.toFloat(), Gdx.graphics.height.toFloat()).apply {
            near = 0.1f; far = 100f
        }
        modelBatch = ModelBatch()
        environment = Environment().apply {
            set(ColorAttribute(ColorAttribute.AmbientLight, 0.72f, 0.72f, 0.78f, 1f))
            add(DirectionalLight().set(0.9f, 0.86f, 0.78f, -1f, -2f, -1f))
        }
        shape = ShapeRenderer()
        hudBatch = SpriteBatch()
        font = BitmapFont()
        buildWorld()
        Gdx.input.inputProcessor = object : InputAdapter() {
            override fun touchDown(screenX: Int, screenY: Int, pointer: Int, button: Int): Boolean {
                if (gameOver) { resetGame(); return true }
                val w = Gdx.graphics.width.toFloat()
                if (screenX > w * 0.78f && screenY > Gdx.graphics.height * 0.62f) {
                    placeTrap()
                } else if (screenX < w * 0.48f && leftPointer == -1) {
                    leftPointer = pointer; leftStartX = screenX.toFloat(); leftStartY = screenY.toFloat()
                    moveX = 0f; moveY = 0f
                } else if (rightPointer == -1) {
                    rightPointer = pointer; lastRightX = screenX.toFloat(); lastRightY = screenY.toFloat()
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
                    lastRightX = screenX.toFloat(); lastRightY = screenY.toFloat()
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
        makeBox(0f, 2f, -10f, 20f, 4f, 0.25f, Color.valueOf("e7d5b7"))
        makeBox(0f, 2f, 10f, 20f, 4f, 0.25f, Color.valueOf("e7d5b7"))
        makeBox(-10f, 2f, 0f, 0.25f, 4f, 20f, Color.valueOf("e7d5b7"))
        makeBox(10f, 2f, 0f, 0.25f, 4f, 20f, Color.valueOf("e7d5b7"))
        // Kitchen island, cupboards and a recognizable fridge.
        makeBox(-3.2f, 1f, -2f, 2.2f, 2f, 0.8f, Color.valueOf("8c573d"))
        makeBox(-5.4f, 1.25f, -5.5f, 1.5f, 2.5f, 1.5f, Color.valueOf("d9e3e8"))
        makeBox(-5.4f, 1.25f, -4.72f, 1.42f, 2.45f, 0.08f, Color.valueOf("b9cbd3"))
        makeBox(3.8f, 0.8f, -3.5f, 2.8f, 1.6f, 1f, Color.valueOf("9a6545"))
        // Table and chairs.
        makeBox(3f, 1.35f, 3f, 3.4f, 0.18f, 2f, Color.valueOf("70452f"))
        for (x in listOf(1.7f, 4.3f)) makeBox(x, 0.65f, 3f, 0.18f, 1.3f, 0.18f, Color.valueOf("70452f"))
        tom = makeSphere(tomPosition.x, tomPosition.y, tomPosition.z, 1.15f, Color.valueOf("777b83"))
        makeSphere(tomPosition.x - 0.43f, 1.12f, tomPosition.z, 0.42f, Color.valueOf("777b83"))
        makeSphere(tomPosition.x + 0.43f, 1.12f, tomPosition.z, 0.42f, Color.valueOf("777b83"))
        val spots = listOf(Vector3(-2f, 0.35f, 2f), Vector3(5f, 0.35f, -1f), Vector3(-6f, 0.35f, 5f))
        cheese.addAll(spots)
        spots.forEach { makeSphere(it.x, it.y, it.z, 0.48f, Color.valueOf("ffd447")) }
        // windows
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

    private fun placeTrap() {
        if (cheeseCount > 0 && !trapPlaced) {
            trapPlaced = true
            cheeseCount--
            val p = Vector3(player.x + forward.x * 1.6f, 0.12f, player.z + forward.z * 1.6f)
            makeBox(p.x, p.y, p.z, 0.9f, 0.18f, 0.65f, Color.LIGHT_GRAY)
            makeBox(p.x, p.y + 0.18f, p.z, 0.62f, 0.08f, 0.08f, Color.DARK_GRAY)
        }
    }

    private fun updateGame(delta: Float) {
        if (gameOver) return
        val dt = delta.coerceAtMost(0.05f)
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

        if (trapped > 0f) trapped -= dt else {
            val chase = Vector3(player.x - tomPosition.x, 0f, player.z - tomPosition.z)
            if (chase.len2() > 0.01f) tomPosition.mulAdd(chase.nor(), 1.25f * dt)
            tom.transform.setToTranslation(tomPosition)
        }
        if (trapPlaced && tomPosition.dst(player) < 2.0f) trapped = 4f
        if (tomPosition.dst(player) < 0.85f) gameOver = true

        val iterator = cheese.iterator()
        while (iterator.hasNext()) {
            val c = iterator.next()
            if (player.dst(c) < 1.0f) { iterator.remove(); cheeseCount++ }
        }
        if (cheeseCount >= 3) won = true
        updateCamera()
    }

    override fun render() {
        val dt = Gdx.graphics.deltaTime
        updateGame(dt)
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
        shape.projectionMatrix = camera.combined
        shape.begin(ShapeRenderer.ShapeType.Filled)
        shape.color = Color(0f, 0f, 0f, 0.28f)
        shape.circle(92f, 92f, 54f)
        shape.circle(Gdx.graphics.width - 95f, 95f, 54f)
        shape.color = Color(1f, 1f, 1f, 0.35f)
        shape.circle(92f + moveX * 28f, 92f - moveY * 28f, 19f)
        shape.end()
        hudBatch.begin()
        font.color = Color.WHITE
        font.data.setScale(1.5f)
        font.draw(hudBatch, "CHEESE: $cheeseCount / 3", 22f, Gdx.graphics.height - 24f)
        font.data.setScale(1.0f)
        font.draw(hudBatch, "LEFT: MOVE     RIGHT: LOOK     TRAP: bottom-right", 22f, 30f)
        if (won) font.draw(hudBatch, "YOU WIN! Tap to restart", Gdx.graphics.width / 2f - 100f, Gdx.graphics.height / 2f)
        if (gameOver) font.draw(hudBatch, "TOM CAUGHT YOU! Tap to restart", Gdx.graphics.width / 2f - 145f, Gdx.graphics.height / 2f)
        if (cheeseCount > 0 && !trapPlaced) font.draw(hudBatch, "Tap trap button to distract Tom", 22f, 62f)
        hudBatch.end()
    }

    private fun resetGame() {
        gameOver = false; won = false; cheeseCount = 0; trapped = 0f; trapPlaced = false
        player.set(0f, 1.65f, 6f); tomPosition.set(0f, 0.65f, -5f)
        cheese.clear()
        cheese.addAll(listOf(Vector3(-2f, 0.35f, 2f), Vector3(5f, 0.35f, -1f), Vector3(-6f, 0.35f, 5f)))
        // Full scene reset is handled by recreating the application for this prototype.
        Gdx.app.exit()
    }

    override fun resize(width: Int, height: Int) {
        camera.viewportWidth = width.toFloat(); camera.viewportHeight = height.toFloat(); camera.update()
    }

    override fun dispose() {
        modelBatch.dispose(); shape.dispose(); hudBatch.dispose(); font.dispose()
        models.forEach { it.dispose() }
    }
}
