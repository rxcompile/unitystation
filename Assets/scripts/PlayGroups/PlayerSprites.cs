﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UI;
using System.Collections.Generic;

namespace PlayGroup
{
	public class PlayerSprites: NetworkBehaviour
	{
		[SyncVar(hook = "FaceDirection")]
		public Vector2 currentDirection;

        private bool initSync = false;
		public PlayerMove playerMove;
		private Dictionary<string, ClothingItem> clothes = new Dictionary<string, ClothingItem>();

		void Awake()
		{
			foreach (var c in GetComponentsInChildren<ClothingItem>()) {
				clothes[c.name] = c;
			}
		}

		public override void OnStartServer(){
			FaceDirection(Vector2.down);
			base.OnStartServer();
		}

		public override void OnStartClient(){
			StartCoroutine(WaitForLoad());
			base.OnStartClient();
		}

		IEnumerator WaitForLoad(){
			yield return new WaitForSeconds(2f);
			FaceDirection(currentDirection);
		}

		public void AdjustSpriteOrders(int offsetOrder){
			foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()) {
				int newOrder = s.sortingOrder;
				newOrder += offsetOrder;
				s.sortingOrder = newOrder;
			}
		}

		[Command]
		public void CmdChangeDirection(Vector2 direction){
            FaceDirection(direction); 
		}
		//turning character input and sprite update
		public void FaceDirection(Vector2 direction)
		{
            SetDir(direction);
		}
         
		public void SetDir(Vector2 direction)
		{
            if (playerMove.isGhost)
                return;
            if (currentDirection != direction || !initSync) {
                if (!initSync)
                    initSync = true;
                
				foreach (var c in clothes.Values) {
					c.Direction = direction;
				}
                currentDirection = direction;
			}
		}
	}
}