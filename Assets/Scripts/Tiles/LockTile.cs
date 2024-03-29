using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTile : Tile
{
    [SerializeField] private int keyIndex;
    
    public int KeyIndex => keyIndex;
    
    private GridManager gridManager;
    private PlayerMovement player;
    
    public event System.Action OnLockOpened;
    public event System.Action OnLockClosed;
    
    private bool isLocked = true;

    private KeyTile[] keys;
    
    
    private void Start()
    {
        player = Utils.GetAllObjectsOnlyInScene<PlayerMovement>()[0];
        gridManager = FindObjectOfType<GridManager>();
        keys = Utils.GetAllObjectsOnlyInScene<KeyTile>();
    }
    
    public override bool TryWalkingOnTile(Direction direction, bool isPlayer = false)
    {
        StartCoroutine(CheckForKeyCoroutine());
        return true;
    }

    private IEnumerator CheckForKeyCoroutine()
    {
        yield return null;
        yield return new WaitUntil(() => !player.IsMoving);
        CheckForKey();
    }

    public void CheckForKey()
    {
        List<GameObject> objects = gridManager.GetObjectsAtPosition(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        foreach (GameObject obj in objects)
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile != null)
            {
                if (tile is KeyTile keyTile)
                {
                    if (keyTile.KeyIndex == keyIndex)
                    {
                        OpenLock();
                        keyTile.Activate();
                        return;
                    }
                }
            }
        }
        CloseLock();
        foreach (KeyTile keyTile in keys)
        {
            if (keyTile.KeyIndex == keyIndex)
            {
                keyTile.Deactivate();
            }
        }
    }
    
    public void OpenLock()
    {
        Debug.Log("Lock opened");
        if (!isLocked)
        {
            return;
        }
        isLocked = false;
        OnLockOpened?.Invoke();
    }
    
    public void CloseLock()
    {
        Debug.Log("Lock closed");
        if (isLocked)
        {
            return;
        }
        isLocked = true;
        OnLockClosed?.Invoke();
    }
}
