using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerBoard : MonoBehaviour
{
    #region Singleton
    // "Instance" keyword variable can be accessed everywhere
    public static CheckerBoard Instance;

    private void Awake()
    {
        // Set 'this' class as the first instance
        Instance = this;
    }
    #endregion
    [Header("Game Logic")]
    public Piece[,] pieces = new Piece[8, 8];// 2D Array - https://www.cs.cmu.edu/~mrmiller/15-110/Handouts/arrays2D.pdf
    public GameObject whitePiecePrefab, blackPiecePrefab; // Prefabs to spawn
    // Offset values of the board
    public Vector3 boardOffset = new Vector3(-4f, 0f, -4f);
    public Vector3 pieceOffset = new Vector3(.5f, .125f, .5f);
    public LayerMask hitLayers;
    public float rayDistance = 25f;

    private bool isWhite; // Is the current character white?
    private bool isWhiteRun; // Is it white's turn?
    private bool hasKilled; // Has the player killed a piece?
    private Piece selectedPiece; // Current selected piece
    private List<Piece> forcedPieces; // List storing the pieces that are forced moves
    private Vector2 mouseOver; // Mouse over value
    private Vector2 startDrag; // Position of start drag
    private Vector2 endDrag; // Position of end drag

    void MovePiece(Piece pieceToMove, int x, int y)
    {
        // Move the Piece to world coordinate using x amd y + offsets
        Vector3 coordinate = new Vector3(x, 0f, y);
        pieceToMove.transform.position = coordinate + boardOffset + pieceOffset;
    }
    void GeneratePiece(bool isWhite, int x, int y)
    {
        GameObject prefab = isWhite ? whitePiecePrefab : blackPiecePrefab; // Which prefab is the piece?
        GameObject clone = Instantiate(prefab) as GameObject; // Instantiate the prefab 
        clone.transform.SetParent(transform); // Make checkerboard the parent of new piece
        Piece pieceScript = clone.GetComponent<Piece>(); // Get the "Piece" Component from clone ('Piece' needs to be attached to prefabs)
        pieces[x, y] = pieceScript; // Add piece component to array
        MovePiece(pieceScript, x, y); // Move the piece to correct world position

    }



    // Generate the board pieces
    void Generateboard()
    {
        // Generate white team
        for (int y = 0; y < 3; y++)
        {
            // If the remainder of /2 is zero, it is true
            // % = modulo - https://www.dotnetperls.com/modulo
            bool oddRow = y % 2 == 0;
            //loop throug 9 and skip 2 every time
            for (int x = 0; // Initializer
                 x < 8;  // Condition
                 x += 2) // Incrementer / Iteration
                         // For Loop - https://www.tutorialspoint.com/csharp/csharp_for_loop.htm
            {
                // Generate pice here
                int desiredX = oddRow ? x : x + 1;
                int desiredY = y;
                GeneratePiece(true, desiredX, desiredY);

            }
        }
        // Generate black team
        for (int y = 7; y > 4; y--) // Go backward from 7
        {
            bool oddRow = y % 2 == 0;
            for (int x = 0; x < 8; x += 2)
            {
                int desiredX = oddRow ? x : x + 1;
                int desiredY = y;
                GeneratePiece(false, desiredX, desiredY);

            }

        }
    }
    void UpdateMouseOver()
    {
        // Does the main camera exist?
        if (Camera.main == null)
        {
            Debug.Log("unable to find main camera");
            // Exit the whole function
            return;
        }
        {
            // Generate ray from mouse input to world
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Perform raycast
            if (Physics.Raycast(camRay, out hit, rayDistance, hitLayers))
            {
                // Convert world position to an array index (by converting to int aswell)
                mouseOver.x = (int)(hit.point.x - boardOffset.x);
                mouseOver.y = (int)(hit.point.z - boardOffset.z);
            }
            else
            {
                // '-1' means nothing was selected
                mouseOver.y = -1;
                mouseOver.x = -1;
            }
        }
    }
    void UpdatePieceDrag(Piece pieceToDrag)
    {
        if (Camera.main == null)
        {
            Debug.Log("unable to find Main Camera");
            // Exit the function
            return;
        }
        // Generate ray from mouse input to world
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Perform raycast
        if (Physics.Raycast(camRay, out hit, rayDistance, hitLayers))
        {
            // Start dragging the piece and move it just above the cursor
            pieceToDrag.transform.position = hit.point + Vector3.up;
        }
    }
    void SelectPiece(int x, int y)
    {
        // Check if x and y is outside of bounds of pieces array
        // x < 0 || x >= array length || y 
        if (x < 0 || x >= pieces.GetLength(0) ||
            y < 0 || y >= pieces.GetLength(1))
        {
            // return - exit function
            return;
        }


        Piece p = pieces[x, y];
        if (p != null && p.isWhite == isWhite)
        {
            selectedPiece = p;
            startDrag = mouseOver;
        }
    }
    void TryMove(int x1, int y1, int x2, int y2)
    {
        if (x1 < 0 || x1 >= pieces.GetLength(0) ||
            x2 < 0 || x2 >= pieces.GetLength(0) ||
            y1 < 0 || y1 >= pieces.GetLength(1) ||
            y2 < 0 || y2 >= pieces.GetLength(1))
        {
            return;
        }
        if (selectedPiece != null)
        {
            MovePiece(selectedPiece, x2, y2);

            Piece temp = pieces[x1, y1]; // save origina to temp    
            Piece newSlot = pieces[x2, y2];
            if (newSlot)
            {
                MovePiece(newSlot, x1, y1);
            }
            pieces[x1, y1] = pieces[x2, y2]; //re
            pieces[x2, y2] = temp;
            // selectedPiece = null;
            selectedPiece = null;
        }
    }
    // Use this for initialization
    void Start()
    {
        Generateboard();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseOver();
        // Is is white's turn or black's turn?
        if (isWhite ? isWhiteRun : !isWhiteRun)
        {
            // Convert coordinates to int (again to be sure)
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            // Select the piece - void SelectPiece(int x , int y)

            // If mousebutton down
            if (Input.GetMouseButtonDown(0))
            {
                // SelectPiece(x, y)
                SelectPiece(x, y);
            }

            // Is there a selectedPiece currently?
            if (selectedPiece != null)
            {
                // Update the drag position
                UpdatePieceDrag(selectedPiece);
                if (Input.GetMouseButtonUp(0))
                {
                    TryMove((int)startDrag.x, (int)startDrag.y, x, y);


                }
            }
            //  MovePiece(selectedPiece, x, y);
        }
    }
}
