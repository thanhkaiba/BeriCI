/*using DG.Tweening;
using Piratera.Config;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Piratera.GUI
{

    public class PickerWheel : MonoBehaviour
    {

        [Header("References :")]
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private Transform linesParent;

        [Space]
        [SerializeField] private Transform PickerWheelTransform;
        [SerializeField] private Transform wheelCircle;
        [SerializeField] private GameObject wheelPiecePrefab;
        [SerializeField] private Transform wheelPiecesParent;



        [Space]
        [Header("Picker wheel settings :")]
        [Range(1, 20)] public int spinDuration = 8;
        [SerializeField] [Range(.2f, 2f)] private float wheelSize = 1f;

        [Space]
        [SerializeField]
        private Sprite[] gifts;

        private string[] giftString;
        private long lastRoll;

        [Space]
        [Header("Picker wheel pieces :")]
        public WheelPiece[] wheelPieces;


        // Events
        private UnityAction onSpinStartEvent;
        private UnityAction<WheelPiece> onSpinEndEvent;


        private bool spinning = false;



        private Vector2 pieceMinSize = new Vector2(81f, 146f);
        private Vector2 pieceMaxSize = new Vector2(144f, 213f);
        private int piecesMin = 2;
        private int piecesMax = 12;

        private float pieceAngle;
        private float halfPieceAngle;
        private float halfPieceAngleWithPaddings;


        private void Start()
        {
            pieceAngle = 360 / wheelPieces.Length;
            halfPieceAngle = pieceAngle / 2f;
            halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);

            Generate();

            CalculateWeightsAndIndices();

        }





        private void Generate()
        {
            wheelPiecePrefab = InstantiatePiece();

            RectTransform rt = wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
            float pieceWidth = Mathf.Lerp(pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
            float pieceHeight = Mathf.Lerp(pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pieceWidth);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pieceHeight);

            giftString = PirateWheelData.Instance.getPrize().Split(char.Parse(":"));
            for (int i = 0; i < giftString.Length - 1; i++)
            {
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
                DrawPiece(i);

            Destroy(wheelPiecePrefab);
        }

        public void DrawPiece(int index)
        {
            WheelPiece piece = wheelPieces[index];
            Transform pieceTrns = InstantiatePiece().transform.GetChild(0);

            pieceTrns.GetChild(0).GetComponent<Image>().sprite = piece.Icon;
            pieceTrns.GetChild(1).GetComponent<Text>().text = piece.Label;
            pieceTrns.GetChild(2).GetComponent<Text>().text = piece.Amount.ToString();

            //Line
            Transform lineTrns = Instantiate(linePrefab, linesParent.position, Quaternion.identity, linesParent).transform;
            lineTrns.RotateAround(wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);

            pieceTrns.RotateAround(wheelPiecesParent.position, Vector3.back, pieceAngle * index);
        }

        private GameObject InstantiatePiece()
        {
            return Instantiate(wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent);
        }



        public long getTimeRemaining()
        {
            return GlobalConfigs.PirateWheelConfig.timeCycle * 1000 - GameTimeMgr.GetCurrentTime() + lastRoll;
        }



        private void Spin(int index)
        {
            if (!spinning)
            {
                spinning = true;
                if (onSpinStartEvent != null)
                    onSpinStartEvent.Invoke();

                //int index = GetRandomPieceIndex () ;
                WheelPiece piece = wheelPieces[index];



                float angle = -(pieceAngle * index);

                float rightOffset = (angle - halfPieceAngleWithPaddings) % 360;
                float leftOffset = (angle + halfPieceAngleWithPaddings) % 360;

                float randomAngle = Random.Range(leftOffset, rightOffset);

                Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * spinDuration);

                float prevAngle, currentAngle;
                prevAngle = currentAngle = wheelCircle.eulerAngles.z;

                bool isIndicatorOnTheLine = false;

                wheelCircle
                .DORotate(targetRotation, spinDuration, RotateMode.Fast)
                .SetEase(Ease.InOutQuart)
                .OnUpdate(() =>
                {
                    float diff = Mathf.Abs(prevAngle - currentAngle);
                    if (diff >= halfPieceAngle)
                    {
                        prevAngle = currentAngle;
                        isIndicatorOnTheLine = !isIndicatorOnTheLine;
                    }
                    currentAngle = wheelCircle.eulerAngles.z;
                })
                .OnComplete(() =>
                {
                    spinning = false;
                    if (onSpinEndEvent != null)
                        onSpinEndEvent.Invoke(piece);

                    onSpinStartEvent = null;
                    onSpinEndEvent = null;
                });

            }
        }





        private void CalculateWeightsAndIndices()
        {
            for (int i = 0; i < wheelPieces.Length; i++)
            {
                WheelPiece piece = wheelPieces[i];


                //add index :
                piece.Index = i;


            }
        }




        private void OnValidate()
        {
            if (PickerWheelTransform != null)
                PickerWheelTransform.localScale = new Vector3(wheelSize, wheelSize, 1f);
        }
    }
}*/