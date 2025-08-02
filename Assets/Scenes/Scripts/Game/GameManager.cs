// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;


// public class GameManager : MonoBehaviour
// {
//     public static GameManager Instance;
    
//     [Header("Tilemaps")]
//     public Tilemap wallTilemap; // For snake segments (walls)
//     public Tilemap boxTilemap;
//     public Tilemap playerTilemap;
//     public Tilemap targetTilemap;
    
//     [Header("Tiles")]
//     public TileBase wallTile;
//     public TileBase boxTile;
//     public TileBase playerTile;
//     public TileBase targetTile;
//     public TileBase pushableWallTile; // Special snake segments that can have boxes pushed onto them
    
//     private Snake snake;
//     private MainCharacter player;
//     private List<Vector2Int> boxes = new List<Vector2Int>();
//     private List<Vector2Int> targets = new List<Vector2Int>();
//     private List<Vector2Int> pushableWalls = new List<Vector2Int>();
    
//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }
    
//     private void Start()
//     {
//         LoadLevel(1); 
//     }
    
//     public void LoadLevel(int levelNumber)
//     {
//         ClearLevel();

//         if (levelNumber == 1)
//         {
//             // // Create snake (position, length, initial direction)
//             // snake = new Snake(new Vector2Int(3, 5), 4, Vector2Int.right);
            
//             // // Set some segments as pushable (can have boxes pushed onto them)
//             // pushableWalls.Add(new Vector2Int(3, 5));
//             // pushableWalls.Add(new Vector2Int(4, 5));
            
//             // // Create player
//             // player = new MainCharacter(new Vector2Int(2, 2));
            
//             // // Create boxes and targets
//             // boxes.Add(new Vector2Int(3, 3));
//             // targets.Add(new Vector2Int(7, 3));
//         }
        
//         UpdateVisuals();
//     }
    
//     private void ClearLevel()
//     {
//         wallTilemap.ClearAllTiles();
//         boxTilemap.ClearAllTiles();
//         playerTilemap.ClearAllTiles();
//         targetTilemap.ClearAllTiles();
        
//         boxes.Clear();
//         targets.Clear();
//         pushableWalls.Clear();
//     }
    
//     private void Update()
//     {
//         HandleInput();
//     }
    
//     private void HandleInput()
//     {
//         // Snake controls (WASD)
//         if (Input.GetKeyDown(KeyCode.W)) snake.SetDirection(Vector2Int.up);
//         if (Input.GetKeyDown(KeyCode.S)) snake.SetDirection(Vector2Int.down);
//         if (Input.GetKeyDown(KeyCode.A)) snake.SetDirection(Vector2Int.left);
//         if (Input.GetKeyDown(KeyCode.D)) snake.SetDirection(Vector2Int.right);
        
//         // Player controls (Arrows)
//         if (Input.GetKeyDown(KeyCode.UpArrow)) TryMovePlayer(Vector2Int.up);
//         if (Input.GetKeyDown(KeyCode.DownArrow)) TryMovePlayer(Vector2Int.down);
//         if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMovePlayer(Vector2Int.left);
//         if (Input.GetKeyDown(KeyCode.RightArrow)) TryMovePlayer(Vector2Int.right);
//     }
    
//     private void TryMovePlayer(Vector2Int direction)
//     {
//         Vector2Int newPosition = player.Position + direction;
        
//         // Check what's at the new position
//         if (IsPositionWalkable(newPosition))
//         {
//             player.Move(newPosition);
//         }
//         else if (boxes.Contains(newPosition))
//         {
//             // Try to push the box
//             Vector2Int boxNewPosition = newPosition + direction;
//             if (IsPositionPushable(boxNewPosition))
//             {
//                 boxes.Remove(newPosition);
//                 boxes.Add(boxNewPosition);
//                 player.Move(newPosition);
//                 CheckWinCondition();
//             }
//         }
        
//         UpdateVisuals();
//     }
    
//     private bool IsPositionWalkable(Vector2Int position)
//     {
//         // Check boundaries (you might want to set these differently)
//         if (position.x < 0 || position.y < 0 || position.x >= 10 || position.y >= 10)
//             return false;
            
//         // Check if position is occupied by wall (non-pushable snake segment)
//         if (snake.IsPositionOccupied(position) && !pushableWalls.Contains(position))
//             return false;
            
//         // Check if position has another box
//         if (boxes.Contains(position))
//             return false;
            
//         return true;
//     }
    
//     private bool IsPositionPushable(Vector2Int position)
//     {
//         // Check boundaries
//         if (position.x < 0 || position.y < 0 || position.x >= 10 || position.y >= 10)
//             return false;
            
//         // Position is pushable if it's empty or a pushable wall
//         if (snake.IsPositionOccupied(position))
//             return pushableWalls.Contains(position);
            
//         return !boxes.Contains(position);
//     }
    
//     public void UpdateVisuals()
//     {
//         ClearLevel();
        
//         // Draw snake (walls)
//         foreach (var segment in snake.GetBody())
//         {
//             wallTilemap.SetTile(
//                 new Vector3Int(segment.x, segment.y, 0),
//                 pushableWalls.Contains(segment) ? pushableWallTile : wallTile
//             );
//         }
        
//         // Draw player
//         playerTilemap.SetTile(new Vector3Int(player.Position.x, player.Position.y, 0), playerTile);
        
//         // Draw boxes
//         foreach (var box in boxes)
//         {
//             boxTilemap.SetTile(new Vector3Int(box.x, box.y, 0), boxTile);
//         }
        
//         // Draw targets
//         foreach (var target in targets)
//         {
//             targetTilemap.SetTile(new Vector3Int(target.x, target.y, 0), targetTile);
//         }
//     }
    
//     private void CheckWinCondition()
//     {
//         foreach (var target in targets)
//         {
//             if (!boxes.Contains(target)) return;
//         }
        
//         Debug.Log("Level Complete!");
//         LoadLevel(2); 
//     }
// }
