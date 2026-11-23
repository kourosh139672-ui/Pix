plugins {
    id("com.android.application")
}

dependencies {
    implementation("androidx.webkit:webkit:1.12.1")
}
android {
    namespace = "com.kourosh.droidify"
    compileSdk = 35
    defaultConfig {
        applicationId = "com.kourosh.droidify"
        minSdk = 23
        targetSdk = 28
        versionCode = 1
        versionName = "1.0"
    }
}
