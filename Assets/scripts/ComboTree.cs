using UnityEngine;
using System.Collections;

public class InputTree {
	/*
	private Action idleAction; // Root
	private Action currentAction; // Can't be null
	
	public InputTree() {
		// Create actions
		idleAction = new Action("idle");
		
		Action walk = new Action("walk");
		Action jump = new Action("jump");
		Action grab = new Action("grab");	
		
		Action punch = new Action("punch");
		Action secondPunch = new Action("secondPunch");
		Action elbow = new Action("elbow");
		Action knee = new Action("knee");
		Action breaker = new Action("breaker");
		
		Action jumpKick = new Action("jumpKick");
		
		Action grabKick = new Action("grabKick");
		Action grabThrow = new Action("grabThrow");
		
		// Wire actions
		idleAction.AddTransition(walk);
		idleAction.AddTransition(jump);
		idleAction.AddTransition(punch);
		idleAction.AddTransition(grab);

		walk.AddTransition(jump);
		walk.AddTransition(punch);
		
		jump.AddTransition(jumpKick);
		
		punch.AddTransition(secondPunch);
		secondPunch.AddTransition(elbow);
		elbow.AddTransition(knee);
		knee.AddTransition(breaker);
		
		grab.AddTransition(grabKick);
		grab.AddTransition(walk);
		grab.AddTransition(jump);
		
		grabKick.AddTransition(grabThrow);
		
		
		currentAction = idleAction;
	}
	
	public void Update() {
		
		if(currentAction.Transition != null)
			currentAction = currentAction.Transition;
	}
	
	private class Action {		
		private IList<Action> parents;
		private IDictionary<string, Action> transition;
		
		public Action ExitTransition {get;set;}
		public string Name { get; set; }
		public Action CallingParent { get; set; }
		public Action Transition { get; set; }
		
		public float duration;
		public float looping;
		
		
		public Action(string name) {
			this.name = name;
			parents = new <Action>();
			transition = new Hashtable<string, Action>();
		}
		
		public void AddTransition(Action transition) {
			transitions.Put(transition.Name, transition);
			transition.parents.Put(Name, this);
		}
		
		public Action GetTransition(string actionName) {
			return transitions.Get(actionName);
		}	
	}*/
}

