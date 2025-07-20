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
   
[Serializable]
[System.Runtime.InteropServices.ComVisible(false)]
[DebuggerDisplay("Count = {Count}")]        
public class MyKeyedCollection<TKey,TItem>: Collection<TItem>
{
    const int defaultThreshold = 0;

    IEqualityComparer<TKey> comparer;
    [SerializeField]
    SerializableDictionary<TKey,TItem> dict = new ();
    int keyCount;
    int threshold;
    Func<TItem, TKey> keyMapper;

    public MyKeyedCollection(Func<TItem, TKey> keyMapper) : this()
    {
        this.keyMapper = keyMapper;
    }
    protected MyKeyedCollection(): this(null, defaultThreshold) {}

    protected MyKeyedCollection(IEqualityComparer<TKey> comparer): this(comparer, defaultThreshold) {}


    protected MyKeyedCollection(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold) {
        if (comparer == null) { 
            comparer = EqualityComparer<TKey>.Default;
        }

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

    public TItem this[TKey key] {
        get {
            if( key == null) {
                // ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            if (dict != null) {
                return dict[key];
            }

            foreach (TItem item in Items) {
                if (comparer.Equals(GetKeyForItem(item), key)) return item;
            }

            // ThrowHelper.ThrowKeyNotFoundException();
            throw new KeyNotFoundException($"Key '{key}' not found in the collection.");
            return default(TItem);
        }
    }

    public bool Contains(TKey key) {
        if( key == null) {
            // ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        }
        
        if (dict != null) {
            return dict.ContainsKey(key);
        }

        if (key != null) {
            foreach (TItem item in Items) {
                if (comparer.Equals(GetKeyForItem(item), key)) return true;
            }
        }
        return false;
    }
    public bool TryGetValue(TKey key, out TItem item) => dict.TryGetValue(key, out item);

    private bool ContainsItem(TItem item) {                        
        TKey key;
        if( (dict == null) || ((key = GetKeyForItem(item)) == null)) {
            return Items.Contains(item);
        }

        TItem itemInDict;
        bool exist = dict.TryGetValue(key, out itemInDict);
        if( exist) {
            return EqualityComparer<TItem>.Default.Equals(itemInDict, item);
        }
        return false;
    }

    public bool Remove(TKey key) {
        if( key == null) {
            // ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        }
        
        if (dict != null) {
            if (dict.ContainsKey(key)) {
                return Remove(dict[key]);
            }

            return false;
        }

        if (key != null) {
            for (int i = 0; i < Items.Count; i++) {
                if (comparer.Equals(GetKeyForItem(Items[i]), key)) {
                    RemoveItem(i);
                    return true;
                }
            }
        }
        return false;
    }
    
    protected IDictionary<TKey,TItem> Dictionary {
        get { return dict; }
    }

    protected void ChangeItemKey(TItem item, TKey newKey) {
        // check if the item exists in the collection
        if( !ContainsItem(item)) {
            // ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_ItemNotExist);
            throw new KeyNotFoundException($"Item '{item}' not found in the collection.");
        }

        TKey oldKey = GetKeyForItem(item);            
        if (!comparer.Equals(oldKey, newKey)) {
            if (newKey != null) {
                AddKey(newKey, item);
            }

            if (oldKey != null) {
                RemoveKey(oldKey);
            }
        }
    }

    protected override void ClearItems() {
        base.ClearItems();
        if (dict != null) {
            dict.Clear();
        }

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
            if (newKey != null && dict != null) {
                dict[newKey] = item;
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
        if (dict != null) {
            dict.Add(key, item);
        }
        else if (keyCount == threshold) {
            CreateDictionary();
            dict.Add(key, item);
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
        dict = new SerializableDictionary<TKey,TItem>();
        foreach (TItem item in Items) {
            TKey key = GetKeyForItem(item);
            if (key != null) {
                dict.Add(key, item);
            }
        }
    }

    private void RemoveKey(TKey key) {
        Contract.Assert(key != null, "key shouldn't be null!");
        if (dict != null) {
            dict.Remove(key);
        }
        else {
            keyCount--;
        }
    }
}