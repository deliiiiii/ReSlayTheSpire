// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
// <OWNER>Microsoft</OWNER>
// 

using System.Collections.ObjectModel;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

[Serializable]
[System.Runtime.InteropServices.ComVisible(false)]
[DebuggerDisplay("Count = {Count}")]        
public class MyKeyedCollection<TKey,TItem>: Collection<TItem>
{
    const int DefaultThreshold = 0;

    IEqualityComparer<TKey> comparer;
    [SerializeField]
    SerializableDictionary<TKey,TItem> Dict = new ();
    int keyCount;
    int threshold;
    Func<TItem, TKey> keyMapper;

    public MyKeyedCollection(Func<TItem, TKey> keyMapper) : this()
    {
        this.keyMapper = keyMapper;
    }
    protected MyKeyedCollection(): this(null, DefaultThreshold) {}


    protected MyKeyedCollection(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold = DefaultThreshold) {
        comparer ??= EqualityComparer<TKey>.Default;

        if (dictionaryCreationThreshold == -1) {
            dictionaryCreationThreshold = int.MaxValue;
        }

        if( dictionaryCreationThreshold < -1) {
            // ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.dictionaryCreationThreshold, ExceptionResource.ArgumentOutOfRange_InvalidThreshold);
            throw new ArgumentOutOfRangeException(nameof(dictionaryCreationThreshold), "Invalid threshold value.");
        }

        this.comparer = comparer;
        this.threshold = dictionaryCreationThreshold;
    }

    TKey GetKeyForItem(TItem item) => keyMapper(item);

    public IEqualityComparer<TKey> Comparer => comparer;
    public ICollection<TKey> Keys => Dict.Keys;

    public TItem this[TKey key] {
        get {
            if( key == null) {
                // ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            if (Dict != null) {
                return Dict[key];
            }

            foreach (TItem item in Items) {
                if (comparer.Equals(GetKeyForItem(item), key)) return item;
            }

            // ThrowHelper.ThrowKeyNotFoundException();
            throw new KeyNotFoundException($"Key '{key}' not found in the collection.");
        }
    }

    public bool Contains(TKey key) {
        if( key == null) {
            // ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        }
        
        return Dict?.ContainsKey(key) ?? Items.Any(item => comparer.Equals(GetKeyForItem(item), key));
    }
    public new bool Contains(TItem item) {
        return Contains(GetKeyForItem(item));
    }
    public bool TryGetValue(TKey key, out TItem item) => Dict.TryGetValue(key, out item);

    private bool ContainsItem(TItem item) {                        
        TKey key;
        if( (Dict == null) || ((key = GetKeyForItem(item)) == null)) {
            return Items.Contains(item);
        }

        bool exist = Dict.TryGetValue(key, out var itemInDict);
        return exist && EqualityComparer<TItem>.Default.Equals(itemInDict, item);
    }

    public bool Remove(TKey key) {
        if( key == null) {
            // ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        }
        
        if (Dict != null)
        {
            return Dict.TryGetValue(key, out var value) && base.Remove(value);
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (!comparer.Equals(GetKeyForItem(Items[i]), key)) continue;
            RemoveItem(i);
            return true;
        }
        return false;
    }

    public new bool Remove(TItem item)
    {
        return Remove(GetKeyForItem(item));
    }
    
    protected IDictionary<TKey,TItem> Dictionary => Dict;

    protected void ChangeItemKey(TItem item, TKey newKey) {
        // check if the item exists in the collection
        if( !ContainsItem(item)) {
            // ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_ItemNotExist);
            throw new KeyNotFoundException($"Item '{item}' not found in the collection.");
        }

        TKey oldKey = GetKeyForItem(item);
        if (comparer.Equals(oldKey, newKey)) return;
        if (newKey != null) {
            AddKey(newKey, item);
        }

        if (oldKey != null) {
            RemoveKey(oldKey);
        }
    }

    protected override void ClearItems() {
        base.ClearItems();
        Dict?.Clear();

        keyCount = 0;
    }

    protected override void InsertItem(int index, TItem item) {
        TKey key = GetKeyForItem(item);
        if (key != null) {
            AddKey(key, item);
        }
        base.InsertItem(index, item);
    }

    protected override void RemoveItem(int index) {
        TKey key = GetKeyForItem(Items[index]);
        if (key != null) {
            RemoveKey(key);
        }
        base.RemoveItem(index);
    }

    protected override void SetItem(int index, TItem item) {
        TKey newKey = GetKeyForItem(item);
        TKey oldKey = GetKeyForItem(Items[index]);

        if (comparer.Equals(oldKey, newKey)) {
            if (newKey != null && Dict != null) {
                Dict[newKey] = item;
            }
        }
        else {
            if (newKey != null) {
                AddKey(newKey, item);
            }

            if (oldKey != null) {
                RemoveKey(oldKey);
            }
        }
        base.SetItem(index, item);
    }

    private void AddKey(TKey key, TItem item) {
        if (Dict != null) {
            Dict.Add(key, item);
        }
        else if (keyCount == threshold) {
            CreateDictionary();
            Dict!.Add(key, item);
        }
        else {
            if (Contains(key)) {
                // ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AddingDuplicate);
                throw new ArgumentException($"An item with the same key has already been added: {key}", nameof(key));
            }

            keyCount++;
        }
    }

    private void CreateDictionary() {
        // TODO 
        Dict = new SerializableDictionary<TKey,TItem>();
        foreach (TItem item in Items) {
            TKey key = GetKeyForItem(item);
            if (key != null) {
                Dict.Add(key, item);
            }
        }
    }

    private void RemoveKey(TKey key) {
        Contract.Assert(key != null, "key shouldn't be null!");
        if (Dict != null) {
            Dict.Remove(key);
        }
        else {
            keyCount--;
        }
    }
}