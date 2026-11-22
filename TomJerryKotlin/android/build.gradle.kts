plugins {
    id("com.android.application")
    kotlin("android")
}
android {
    namespace = "org.pix.tomjerry"
    compileSdk = 35
    defaultConfig {
        applicationId = "org.pix.tomjerry"
        minSdk = 23
        targetSdk = 28
        versionCode = 2
        versionName = "0.2.0"
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
    kotlinOptions { jvmTarget = "17" }
}
dependencies {
    implementation(project(":core"))
    implementation("com.badlogicgames.gdx:gdx-backend-android:1.13.5")
    implementation("com.badlogicgames.gdx:gdx-platform:1.13.5:natives-armeabi-v7a")
    implementation("com.badlogicgames.gdx:gdx-platform:1.13.5:natives-arm64-v8a")
}
