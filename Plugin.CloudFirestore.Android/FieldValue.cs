﻿using System;
using System.Linq;

namespace Plugin.CloudFirestore
{
    public partial class FieldValue
    {
        internal Firebase.Firestore.FieldValue ToNative()
        {
            switch (_fieldValueType)
            {
                case FieldValueType.Delete:
                    return Firebase.Firestore.FieldValue.Delete();
                case FieldValueType.ServerTimestamp:
                    return Firebase.Firestore.FieldValue.ServerTimestamp();
                case FieldValueType.ArrayUnion:
                    return Firebase.Firestore.FieldValue.ArrayUnion(_elements?.Select(x => x.ToNativeFieldValue()).ToArray());
                case FieldValueType.ArrayRemove:
                    return Firebase.Firestore.FieldValue.ArrayRemove(_elements?.Select(x => x.ToNativeFieldValue()).ToArray());
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
