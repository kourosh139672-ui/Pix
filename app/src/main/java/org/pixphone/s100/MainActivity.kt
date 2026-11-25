package org.pixphone.s100

import android.app.Activity
import android.os.Bundle
import android.graphics.Color
import android.view.Gravity
import android.widget.LinearLayout
import android.widget.TextView

class MainActivity : Activity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val root = LinearLayout(this).apply {
            orientation = LinearLayout.VERTICAL
            gravity = Gravity.CENTER
            setPadding(32, 48, 32, 48)
            setBackgroundColor(Color.rgb(18, 24, 38))
        }
        val title = TextView(this).apply {
            text = "PixPhone"
            textSize = 34f
            setTextColor(Color.WHITE)
            gravity = Gravity.CENTER
        }
        val device = TextView(this).apply {
            text = "S100 • Prototype 0.1"
            textSize = 18f
            setTextColor(Color.rgb(160, 190, 220))
            gravity = Gravity.CENTER
            setPadding(0, 16, 0, 24)
        }
        val note = TextView(this).apply {
            text = "First interface prototype\\nNot a bootable operating system yet"
            textSize = 16f
            setTextColor(Color.LTGRAY)
            gravity = Gravity.CENTER
        }
        root.addView(title)
        root.addView(device)
        root.addView(note)
        setContentView(root)
    }
}
