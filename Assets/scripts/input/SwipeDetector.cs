﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
	public float minSwipeDistY;
	public float minSwipeDistX;
	public SkyboxCamera skyboxCameraPrefab;

	private Vector3 baseAccel = Vector3.zero;

	private Vector2 startPos;

	private SkyboxCamera skyboxCamera;

	private Ball ball;

	private const float SCALE_SWIPE_MULTIPLIER = 0.1f;
	private const float ACCEL_MULTIPLIER = 12;
	private const float MAX_SWIPE_X = 150;
	private const float MAX_SWIPE_Y = 150;
	private const float MAX_ACCEL_VALUE = 0.4f;

	public float speed = 1.0f;

	public void SetsListener (Ball ball)
	{
		this.ball = ball;
	}

	public void Start ()
	{

		skyboxCamera = Instantiate (skyboxCameraPrefab) as SkyboxCamera;
	}

	void swipe ()
	{
		if (Input.touchCount > 0) {
			Touch touch = Input.touches [0];
			switch (touch.phase) {
			case TouchPhase.Began:
				startPos = touch.position;
				break;
			case TouchPhase.Ended:
				float swipeDistVertical = (new Vector3 (0, touch.position.y, 0) - new Vector3 (0, startPos.y, 0)).magnitude;
				swipeDistVertical = Math.Min (swipeDistVertical, MAX_SWIPE_Y);
				if (swipeDistVertical > minSwipeDistY) {
					float swipeValue = Mathf.Sign (touch.position.y - startPos.y);
					if (swipeValue > 0) {//up swipe
						if (ball != null && ball.gameObject != null) {
							ball.Move (MazeDirection.North, (int)(swipeDistVertical * SCALE_SWIPE_MULTIPLIER));
						}
					} else if (swipeValue < 0)
							//down swipe
							if (ball != null && ball.gameObject != null)
						ball.Move (MazeDirection.South, (int)(swipeDistVertical * SCALE_SWIPE_MULTIPLIER));
				}
				float swipeDistHorizontal = (new Vector3 (touch.position.x, 0, 0) - new Vector3 (startPos.x, 0, 0)).magnitude;
				swipeDistHorizontal = Math.Min (swipeDistHorizontal, MAX_SWIPE_X);
				if (swipeDistHorizontal > minSwipeDistX) {
					float swipeValue = Mathf.Sign (touch.position.x - startPos.x);
					if (swipeValue > 0) {//right swipe
						if (ball != null && ball.gameObject != null)
							ball.Move (MazeDirection.East, (int)(swipeDistHorizontal * SCALE_SWIPE_MULTIPLIER));
					} else if (swipeValue < 0)
							//left swipe
							if (ball != null && ball.gameObject != null)
						ball.Move (MazeDirection.West, (int)(swipeDistHorizontal * SCALE_SWIPE_MULTIPLIER));
				}
				break;
			}
		}
	}

	public void Update ()
	{
		if (ball != null && ball.gameObject != null) {
			if (baseAccel == Vector3.zero)
				baseAccel = Input.acceleration;

			Vector3 accel = Input.acceleration;

			Vector3 delta = Vector3.Cross(accel,-1*baseAccel);

			float cappedX = Math.Sign (accel.x - baseAccel.x) * Math.Min (Math.Abs (accel.x - baseAccel.x), MAX_ACCEL_VALUE);
			float cappedY = Mathf.Sign (delta.x) * Math.Min (Math.Abs (delta.x), MAX_ACCEL_VALUE);

			Debug.Log (cappedX + " - " + cappedY);

			skyboxCamera.OnAcceleration (cappedX, cappedY);

			//fix when device is from bottom to front
			//cappedY = ((Input.acceleration.z>0) ? -1 : 1) * cappedY;

			MazeDirection direction = MazeDirection.East;
			ball.Move (direction, cappedX * ACCEL_MULTIPLIER);

			direction = MazeDirection.North;
			ball.Move (direction, cappedY * ACCEL_MULTIPLIER);

			//swipe ();
		} else {
			baseAccel = Vector3.zero;
		}
	}
}
