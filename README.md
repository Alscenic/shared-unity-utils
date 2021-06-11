# What is it?
A bunch of stuff I reuse in lots of projects. It's all pretty useful in one way or another. Scroll down for features.

# Use it how you want
Just release any edits with the same license.

Entirely my own code

# Requirements
* UnityEngine.InputSystem
  * You should be using it anyway
* Unity
  * yea

# Files (Features)
* Input
  * PlayerInput
    * A unique way to intercept player input. The reason this package requires UnityEngine.Input. Basically routes specific InputActions directly through UnityEvents.
* Audio
  * class AudioData
    * Stores information used to play audio.
  * class Audio
    * Call Audio.Play() with your AudioData to easily play some audio.
  * AudioManager
    * Created automatically at runtime.
* Classes
  * Extensions
    * Various extensions.
  * RangedFloat
    * Composed of two floats, a min and a max, and can return Random.Range(min,max).
  * UnityEventT
    * Various UnityEvent types.
  * VectorCurve
    * Serializable class containing 4 different AnimationCurves, and can be evaluated as Vector2/3/4/Quaternion.
* Misc
  * LazySingletonMonoBehaviour
    * Inherit from this for an easy created-on-demand MonoBehaviour singleton.
  * Destroyer
    * Destroys a GameObject, or itself if one isn't specified.

# How to use it
I might explain everything eventually, but for now, the code is documented so figure it out ig
