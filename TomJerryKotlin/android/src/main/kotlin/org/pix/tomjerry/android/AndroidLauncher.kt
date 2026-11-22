package org.pix.tomjerry.android

import android.graphics.Color
import android.os.Bundle
import android.util.Log
import android.view.Gravity
import android.widget.TextView
import com.badlogic.gdx.backends.android.AndroidApplication
import com.badlogic.gdx.backends.android.AndroidApplicationConfiguration
import org.pix.tomjerry.TomJerryGame

class AndroidLauncher : AndroidApplication() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        try {
            val config = AndroidApplicationConfiguration().apply {
                useImmersiveMode = true
                useAccelerometer = false
                useCompass = false
                numSamples = 0
                useWakelock = true
            }
            initialize(TomJerryGame(), config)
        } catch (error: Throwable) {
            Log.e("TomJerry3D", "Game startup failed", error)
            val message = TextView(this).apply {
                text = "Tom & Jerry 3D\n\nThe game could not start.\n\n" + error.javaClass.simpleName + ": " + (error.message ?: "Unknown error") + "\n\nPlease send this screen to support."
                textSize = 18f
                setTextColor(Color.WHITE)
                setBackgroundColor(Color.rgb(28, 32, 42))
                gravity = Gravity.CENTER
                setPadding(28, 28, 28, 28)
            }
            setContentView(message)
        }
    }
}
