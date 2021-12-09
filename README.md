# ComboTree
 An alternative to Unitys Mecanim for a combo focused combatsystem made using the GraphView and Playables APIs.
# Features
### Mecanim Compatible
ComboTree is not meant to replace but rather extend Mecanim, for parts of your statemachine that you need more controll over.
ComboTree
This allows you keep your more complex state logic whilst still having the option to flexibly transition.
### Runtime Editing
Because ComboTree was made using the Playables API you can easily alter the graph by:
- transitioning to a new animationclip
- transitioning to a new state
- changing the graph completely
### Eventbased Transition
Unlike Mecanim, the Combo Tree transitions are are eventbased and will execute once an external source triggers them.
# Documentation
As per usual, you start by creating a ComboTreeController asset by going into the assetmenu in the project tab.
## States
When you first open the asset you will see the defaul Entry, Any and Exit states.
### Entry State
The Entry State is the default state (the state the ComboAnimator starts in) of your ComboTree and represents you Mecanim animatorcontroller. 
This means that any states connected to it will directly transition from your animatorcontoller and any state it may be in.
### Any State
The Any State has the same function as its Mecanim equivalent, in that any of its transitions will be considered when Transition(string name) is called  (after the ones from the current and next state). Note that you are able to transition out of your default state using the Any State.
### Exit State
The Exit State, like the Entry State, represents the default state in your Combo Tree.
Unlike the Entry State, you are able to create and destroy Exit States (to avoid transition hell), which all point to the same default state.
To create a new Exit State simply rightclick on an empty spot in the editor and choose "New Exit".
### Animation State
The Animation State can, like the Exit State, be created and destroyed and make up the core of a Combo Tree. 
Every Animation State holds a reference to an animationclip which will be played upon entering the state,
and defines a bool "returnToDefault", which when true causes the state to automaticly transition back to the default state 
(this can canceled by simply transitioning to another state).
 To create a new Animation State simply rightclick on an empty spot in the editor and choose "New State".
 To inspect an Animation State simply leftclick on it and it will show up in Unitys default inspector.
 
