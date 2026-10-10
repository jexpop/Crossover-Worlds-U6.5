using System.Collections.Generic;
using CrossoverWorlds.Characters;
using CrossoverWorlds.UI.Utilities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CrossoverWorlds.UI.CharacterSelectionMenu
{

    public class CharacterWheel : MonoBehaviour
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
        [Header("Picker wheel pieces :")]
        public CharacterPiece[] characterPieces;

        // Events
        private UnityAction onSpinStartEvent;
        private UnityAction<CharacterPiece> onSpinEndEvent;


        private bool _isSpinning = false;

        public bool IsSpinning { get { return _isSpinning; } }


        private Vector2 pieceMinSize = new Vector2(81f, 146f);
        private Vector2 pieceMaxSize = new Vector2(144f, 213f);
        private int piecesMin = 2;
        private int piecesMax = 12;

        private float pieceAngle;
        private float halfPieceAngle;
        private float halfPieceAngleWithPaddings;


        private double accumulatedWeight;
        private System.Random rand = new System.Random();

        private List<int> nonZeroChancesIndices = new List<int>();

        public void StartWheel(GameObject[] avaliableCharacters)
        {
            InitializeCharacterPieces(avaliableCharacters);

            pieceAngle = 360 / characterPieces.Length;
            halfPieceAngle = pieceAngle / 2f;
            halfPieceAngleWithPaddings = characterPieces.Length - 8f;

            Generate();

            CalculateWeightsAndIndices();
            if (nonZeroChancesIndices.Count == 0)
                Debug.LogError("You can't set all pieces chance to zero");

        }

        private void InitializeCharacterPieces(GameObject[] avaliableCharacters)
        {
            int idx = -1;

            List<CharacterPiece> characterList = new List<CharacterPiece>();
            foreach(GameObject character in avaliableCharacters)
            {
                CharacterBasicInfo characterInfo= character.GetComponentInChildren<CharacterBasicInfo>();
                CharacterPiece characterPiece = new CharacterPiece();
                characterPiece.icon = characterInfo.avatar;
                characterPiece.label = characterInfo.characterName;
                characterPiece.chance = idx==-1 ? 100f : 0f;

                idx++;
                int idxButton = idx + 1;
                characterPiece.buttonReference = "characterButton" + idxButton;
                characterList.Add(characterPiece);
            }

            characterPieces= characterList.ToArray();
        }

        private void Generate()
        {
            wheelPiecePrefab = InstantiatePiece();

            RectTransform rt = wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
            float pieceWidth = Mathf.Lerp(pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp(piecesMin, piecesMax, characterPieces.Length));
            float pieceHeight = Mathf.Lerp(pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp(piecesMin, piecesMax, characterPieces.Length));
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pieceWidth);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pieceHeight);

            for (int i = 0; i < characterPieces.Length; i++)
                DrawPiece(i);

            Destroy(wheelPiecePrefab);
        }

        private void DrawPiece(int index)
        {
            CharacterPiece piece = characterPieces[index];
            Transform pieceTrns = InstantiatePiece().transform.GetChild(0);
            pieceTrns.parent.name = piece.buttonReference;

            // Initial alpha
            Color color = pieceTrns.GetChild(0).GetComponent<Image>().color;
            float alpha = pieceTrns.parent.name == "characterButton1" ? 1f : 0.5f;
            pieceTrns.GetChild(0).GetComponent<Image>().color = ColorUtilities.SetAlpha(color, alpha);

            pieceTrns.GetChild(0).GetComponent<Image>().sprite = piece.icon;
            pieceTrns.GetChild(1).GetComponent<Text>().text = piece.label;

            //Line
            Transform lineTrns = Instantiate(linePrefab, linesParent.position, Quaternion.identity, linesParent).transform;
            lineTrns.RotateAround(wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);

            pieceTrns.RotateAround(wheelPiecesParent.position, Vector3.back, pieceAngle * index);
        }

        private GameObject InstantiatePiece()
        {
            return Instantiate(wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent);
        }


        public void Spin(int index)
        {
            if (!_isSpinning)
            {
                _isSpinning = true;
                if (onSpinStartEvent != null)
                    onSpinStartEvent.Invoke();

                CharacterPiece piece = characterPieces[index];
                if (piece.chance == 0 && nonZeroChancesIndices.Count != 0)
                {
                    index = nonZeroChancesIndices[Random.Range(0, nonZeroChancesIndices.Count)];
                    piece = characterPieces[index];
                }

                float angle = -(pieceAngle * index);

                float randomAngle = (angle - halfPieceAngleWithPaddings) % 360;

                Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * 0);

                float prevAngle, currentAngle;
                prevAngle = currentAngle = wheelCircle.eulerAngles.z;

                bool isIndicatorOnTheLine = false;

                wheelCircle
                //.DORotate(targetRotation, spinDuration, RotateMode.Fast)
                .DORotate(targetRotation, 0, RotateMode.Fast)
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
                    _isSpinning = false;
                    if (onSpinEndEvent != null)
                        onSpinEndEvent.Invoke(piece);

                    onSpinStartEvent = null;
                    onSpinEndEvent = null;
                });

            }
        }

        public void OnSpinStart(UnityAction action)
        {
            onSpinStartEvent = action;
        }

        public void OnSpinEnd(UnityAction<CharacterPiece> action)
        {
            onSpinEndEvent = action;
        }

        private void CalculateWeightsAndIndices()
        {
            for (int i = 0; i < characterPieces.Length; i++)
            {
                CharacterPiece piece = characterPieces[i];

                //add weights:
                accumulatedWeight += piece.chance;
                piece._weight = accumulatedWeight;

                //add index :
                piece.Index = i;

                //save non zero chance indices:
                if (piece.chance > 0)
                    nonZeroChancesIndices.Add(i);
            }
        }

    }
}