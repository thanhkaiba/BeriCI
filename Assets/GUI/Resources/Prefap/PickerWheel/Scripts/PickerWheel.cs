﻿using UnityEngine ;
using UnityEngine.UI ;
using DG.Tweening ;
using UnityEngine.Events ;
using System.Collections.Generic ;
using Piratera.Network;
using Sfs2X.Entities.Data;
using Piratera.Sound;
using Piratera.Config;

namespace EasyUI.PickerWheelUI {

   public class PickerWheel : MonoBehaviour {

      [Header ("References :")]
      [SerializeField] private GameObject linePrefab ;
      [SerializeField] private Transform linesParent ;

      [Space]
      [SerializeField] private Transform PickerWheelTransform ;
      [SerializeField] private Transform wheelCircle ;
      [SerializeField] private GameObject wheelPiecePrefab ;
      [SerializeField] private Transform wheelPiecesParent ;

      [Space]
      [Header ("Sounds :")]
      [SerializeField] private AudioSource audioSource ;
      [SerializeField] private AudioClip tickAudioClip ;
      [SerializeField] [Range (0f, 1f)] private float volume = .5f ;
      [SerializeField] [Range (-3f, 3f)] private float pitch = 1f ;

      [Space]
      [Header ("Picker wheel settings :")]
      [Range (1, 20)] public int spinDuration = 8 ;
      [SerializeField] [Range (.2f, 2f)] private float wheelSize = 1f ;

        [Space]
        [SerializeField]
        private Sprite[] gifts;

        private string[] giftString;
        private long lastRoll;

        [Space]
      [Header ("Picker wheel pieces :")]
      public WheelPiece[] wheelPieces ;
        

      // Events
      private UnityAction onSpinStartEvent ;
      private UnityAction<WheelPiece> onSpinEndEvent ;


      private bool _isSpinning = false ;

      public bool IsSpinning { get { return _isSpinning ; } }


      private Vector2 pieceMinSize = new Vector2 (81f, 146f) ;
      private Vector2 pieceMaxSize = new Vector2 (144f, 213f) ;
      private int piecesMin = 2 ;
      private int piecesMax = 12 ;

      private float pieceAngle ;
      private float halfPieceAngle ;
      private float halfPieceAngleWithPaddings ;


      private double accumulatedWeight ;
      private System.Random rand = new System.Random () ;

      private List<int> nonZeroChancesIndices = new List<int> () ;

      private void Start () {
         pieceAngle = 360 / wheelPieces.Length ;
         halfPieceAngle = pieceAngle / 2f ;
         halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f) ;

         Generate () ;  

         CalculateWeightsAndIndices () ;
         if (nonZeroChancesIndices.Count == 0)
            Debug.LogError ("You can't set all pieces chance to zero") ;

         NetworkController.AddServerActionListener(OnReceiveServerAction);

            /*
         SetupAudio () ;
         */
        }
        public void setPieces(WheelPiece[] pieces)
        {
            wheelPieces = pieces;
        }
        public void InitWheel()
        {

            pieceAngle = 360 / wheelPieces.Length;
            halfPieceAngle = pieceAngle / 2f;
            halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);

            Generate();

            CalculateWeightsAndIndices();
            if (nonZeroChancesIndices.Count == 0)
                Debug.LogError("You can't set all pieces chance to zero");

            
        }

        private void SetupAudio () {
         audioSource.clip = tickAudioClip ;
         audioSource.volume = volume ;
         audioSource.pitch = pitch ;
      }

      private void Generate () {
         wheelPiecePrefab = InstantiatePiece () ;

         RectTransform rt = wheelPiecePrefab.transform.GetChild (0).GetComponent <RectTransform> () ;
         float pieceWidth = Mathf.Lerp (pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp (piecesMin, piecesMax, wheelPieces.Length)) ;
         float pieceHeight = Mathf.Lerp (pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp (piecesMin, piecesMax, wheelPieces.Length)) ;
         rt.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, pieceWidth) ;
         rt.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, pieceHeight) ;

            giftString = PirateWheelData.Instance.getPrize().Split(char.Parse(":"));
            for (int i = 0; i < giftString.Length - 1; i++)
            {
                Debug.Log(i + ": " + giftString[i]);
                if (i % 2 == 0)
                {
                    wheelPieces[i / 2].Amount = int.Parse(giftString[i]);
                    wheelPieces[i / 2].Amount = int.Parse(giftString[i]);
                    wheelPieces[i / 2].Label = giftString[i + 1];
                    switch (giftString[i + 1])
                    {
                        case "stamina":
                            wheelPieces[i / 2].Icon = gifts[0];
                            break;
                        case "beri":
                            wheelPieces[i / 2].Icon = gifts[1];
                            break;
                    }
                }
            }
            for (int i = 0; i < wheelPieces.Length; i++)
            DrawPiece (i) ;

         Destroy (wheelPiecePrefab) ;
      }

      public void DrawPiece (int index) {
         WheelPiece piece = wheelPieces [ index ] ;
         Transform pieceTrns = InstantiatePiece ().transform.GetChild (0) ;

         pieceTrns.GetChild (0).GetComponent <Image> ().sprite = piece.Icon ;
         pieceTrns.GetChild (1).GetComponent <Text> ().text = piece.Label ;
         pieceTrns.GetChild (2).GetComponent <Text> ().text = piece.Amount.ToString () ;

         //Line
         Transform lineTrns = Instantiate (linePrefab, linesParent.position, Quaternion.identity, linesParent).transform ;
         lineTrns.RotateAround (wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle) ;

         pieceTrns.RotateAround (wheelPiecesParent.position, Vector3.back, pieceAngle * index) ;
      }

      private GameObject InstantiatePiece () {
         return Instantiate (wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent) ;
      }


        private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            if (action == SFSAction.PIRATE_WHEEL)
            {
                if (errorCode != SFSErrorCode.SUCCESS)
                {
                    GameUtils.ShowPopupPacketError(errorCode);
                }
                else
                {
                    Debug.Log("epoch: " + packet.GetLong("reward_epoch") +" timeCycle: "+  GlobalConfigs.PirateWheelConfig.timeCycle + " currentTime: "+ GameTimeMgr.GetCurrentTime());
                    lastRoll = packet.GetLong("reward_epoch");
                    if(GameTimeMgr.GetCurrentTime() - lastRoll > GlobalConfigs.PirateWheelConfig.timeCycle * 1000 || GameTimeMgr.GetCurrentTime() - lastRoll < 0)
                    {
                        string[] part = packet.GetUtfString("reward").Split(char.Parse(":"));
                        GetGiftPiece(part);
                    }
                    else
                    {
                    }
                }
            }
        }
        public long getTimeRemaining()
        {
            return GlobalConfigs.PirateWheelConfig.timeCycle * 1000 - GameTimeMgr.GetCurrentTime() + lastRoll;
        }
        public void OnClose()
        {
            NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        }
        private void GetGiftPiece(string[] p)
        {
            for (int i = 0; i < giftString.Length-1; i++)
            {
                if (i % 2 == 0)
                    if (p[0] == giftString[i] && p[1] == giftString[i + 1])
                    {
                        Spin(i/2);
                        //Debug.Log("Piece:" + i/2 );
                        break;
                    }
            }
        }
        public void SendSpin()
        {
            NetworkController.Send(SFSAction.PIRATE_WHEEL);
        }
        private void Spin (int gift) {
            if (!_isSpinning) {
            _isSpinning = true ;
            if (onSpinStartEvent != null)
               onSpinStartEvent.Invoke () ;

            //int index = GetRandomPieceIndex () ;
            int    index = gift;
            WheelPiece piece = wheelPieces [ index ] ;

            if (piece.Chance == 0 && nonZeroChancesIndices.Count != 0) {
               index = nonZeroChancesIndices [ Random.Range (0, nonZeroChancesIndices.Count) ] ;
               piece = wheelPieces [ index ] ;
            }

            float angle = -(pieceAngle * index) ;

            float rightOffset = (angle - halfPieceAngleWithPaddings) % 360 ;
            float leftOffset = (angle + halfPieceAngleWithPaddings) % 360 ;

            float randomAngle = Random.Range (leftOffset, rightOffset) ;

            Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * spinDuration) ;

            //float prevAngle = wheelCircle.eulerAngles.z + halfPieceAngle ;
            float prevAngle, currentAngle ;
            prevAngle = currentAngle = wheelCircle.eulerAngles.z ;

            bool isIndicatorOnTheLine = false ;

            wheelCircle
            .DORotate (targetRotation, spinDuration, RotateMode.Fast)
            .SetEase (Ease.InOutQuart)
            .OnUpdate (() => {
               float diff = Mathf.Abs (prevAngle - currentAngle) ;
               if (diff >= halfPieceAngle) {
                  if (isIndicatorOnTheLine) {
                     //audioSource.PlayOneShot (audioSource.clip) ;
                  }
                  prevAngle = currentAngle ;
                  isIndicatorOnTheLine = !isIndicatorOnTheLine ;
               }
               currentAngle = wheelCircle.eulerAngles.z ;
            })
            .OnComplete (() => {
               _isSpinning = false ;
               if (onSpinEndEvent != null)
                  onSpinEndEvent.Invoke (piece) ;

               onSpinStartEvent = null ; 
               onSpinEndEvent = null ;
            }) ;

         }
      }

      private void FixedUpdate () {

      }

      public void OnSpinStart (UnityAction action) {
         onSpinStartEvent = action ;
      }

      public void OnSpinEnd (UnityAction<WheelPiece> action) {
         onSpinEndEvent = action ;
      }


      private int GetRandomPieceIndex () {
         double r = rand.NextDouble () * accumulatedWeight ;

         for (int i = 0; i < wheelPieces.Length; i++)
            if (wheelPieces [ i ]._weight >= r)
               return i ;

         return 0 ;
      }

      private void CalculateWeightsAndIndices () {
         for (int i = 0; i < wheelPieces.Length; i++) {
            WheelPiece piece = wheelPieces [ i ] ;

            //add weights:
            accumulatedWeight += piece.Chance ;
            piece._weight = accumulatedWeight ;

            //add index :
            piece.Index = i ;

            //save non zero chance indices:
            if (piece.Chance > 0)
               nonZeroChancesIndices.Add (i) ;
         }
      }




      private void OnValidate () {
         if (PickerWheelTransform != null)
            PickerWheelTransform.localScale = new Vector3 (wheelSize, wheelSize, 1f) ;

         if (wheelPieces.Length > piecesMax || wheelPieces.Length < piecesMin)
            Debug.LogError ("[ PickerWheelwheel ]  pieces length must be between " + piecesMin + " and " + piecesMax) ;
      }
   }
}