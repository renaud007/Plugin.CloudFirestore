﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Plugin.CloudFirestore
{
    public sealed class DocumentObject
    {
        private readonly object _value;

        public DocumentObjectType Type { get; }

        private readonly bool _bool;
        public bool Bool => Type == DocumentObjectType.Bool ? _bool :
            throw new InvalidOperationException("Type is not Bool.");

        private readonly long _long;
        public long Long => Type == DocumentObjectType.Long ? _long :
            throw new InvalidOperationException("Type is not Long.");

        private readonly double _double;
        public double Double => Type == DocumentObjectType.Double ? _double :
            throw new InvalidOperationException("Type is not Double.");

        private readonly string _string;
        public string String => Type == DocumentObjectType.String ? _string :
            throw new InvalidOperationException("Type is not String.");

        private IList<DocumentObject> _list;
        public IList<DocumentObject> List
        {
            get
            {
                if (Type != DocumentObjectType.List)
                {
                    throw new InvalidOperationException("Type is not List.");
                }

                if (_list == null)
                {
                    var fieldInfo = new DocumentFieldInfo<List<DocumentObject>>();
                    _list = (List<DocumentObject>)fieldInfo.DocumentInfo.Create(_value);
                }
                return _list;
            }
        }

        private IDictionary<string, DocumentObject> _dictionary;
        public IDictionary<string, DocumentObject> Dictionary
        {
            get
            {
                if (Type != DocumentObjectType.Dictionary)
                {
                    throw new InvalidOperationException("Type is not Dictionary.");
                }

                if (_dictionary == null)
                {
                    var fieldInfo = new DocumentFieldInfo<Dictionary<string, DocumentObject>>();
                    _dictionary = (Dictionary<string, DocumentObject>)fieldInfo.DocumentInfo.Create(_value);
                }
                return _dictionary;
            }
        }

        private readonly Timestamp _timestamp;
        public Timestamp Timestamp => Type == DocumentObjectType.Timestapm ? _timestamp :
            throw new InvalidOperationException("Type is not Timestapm.");

        private readonly byte[] _bytes;
        public byte[] Bytes => Type == DocumentObjectType.Bytes ? _bytes :
             throw new InvalidOperationException("Type is not Bytes.");

        private readonly GeoPoint _geoPoint;
        public GeoPoint GeoPoint => Type == DocumentObjectType.GeoPoint ? _geoPoint :
            throw new InvalidOperationException("Type is not GeoPoint.");

        private readonly IDocumentReference _documentReference;
        public IDocumentReference DocumentReference => Type == DocumentObjectType.DocumentReference ? _documentReference :
            throw new InvalidOperationException("Type is not DocumentReference.");

        public object Value => Type switch
        {
            DocumentObjectType.Null => null,
            DocumentObjectType.Bool => Bool,
            DocumentObjectType.Long => Long,
            DocumentObjectType.Double => Double,
            DocumentObjectType.String => String,
            DocumentObjectType.List => List,
            DocumentObjectType.Dictionary => Dictionary,
            DocumentObjectType.Timestapm => Timestamp,
            DocumentObjectType.Bytes => Bytes,
            DocumentObjectType.GeoPoint => GeoPoint,
            DocumentObjectType.DocumentReference => DocumentReference,
            _ => throw new InvalidOperationException()
        };

        public DocumentObject()
        {
            Type = DocumentObjectType.Null;
        }

        public DocumentObject(bool value)
        {
            _bool = value;
            Type = DocumentObjectType.Bool;
        }

        public DocumentObject(long value)
        {
            _long = value;
            Type = DocumentObjectType.Long;
        }

        public DocumentObject(double value)
        {
            _double = value;
            Type = DocumentObjectType.Double;
        }

        public DocumentObject(string value)
        {
            _string = value;
            Type = DocumentObjectType.String;
        }

        public DocumentObject(IList<DocumentObject> value)
        {
            _list = value;
            Type = DocumentObjectType.List;
        }

        public DocumentObject(IDictionary<string, DocumentObject> value)
        {
            _dictionary = value;
            Type = DocumentObjectType.Dictionary;
        }

        public DocumentObject(Timestamp value)
        {
            _timestamp = value;
            Type = DocumentObjectType.Timestapm;
        }

        public DocumentObject(byte[] value)
        {
            _bytes = value;
            Type = DocumentObjectType.Bytes;
        }

        public DocumentObject(GeoPoint value)
        {
            _geoPoint = value;
            Type = DocumentObjectType.GeoPoint;
        }

        public DocumentObject(IDocumentReference value)
        {
            _documentReference = value;
            Type = DocumentObjectType.DocumentReference;
        }

        private DocumentObject(object value, DocumentObjectType type)
        {
            _value = value;
            Type = type;
        }

        internal static DocumentObject CreateAsList(object value) => new DocumentObject(value, DocumentObjectType.List);
        internal static DocumentObject CreateAsDictionary(object value) => new DocumentObject(value, DocumentObjectType.Dictionary);

        internal object GetFieldValue(IDocumentFieldInfo fieldInfo)
        {
            if (fieldInfo?.ConvertFrom(this) is (true, var result))
            {
                return result;
            }

            var fieldType = fieldInfo?.NullableUnderlyingType;

            return Type switch
            {
                DocumentObjectType.Null => null,
                DocumentObjectType.Bool when fieldType == null => Bool,
                DocumentObjectType.Bool => Convert.ChangeType(Bool, fieldType),
                DocumentObjectType.Long when fieldType == null => Long,
                DocumentObjectType.Long => Convert.ChangeType(Long, fieldType),
                DocumentObjectType.Double when fieldType == null => Double,
                DocumentObjectType.Double => Convert.ChangeType(Double, fieldType),
                DocumentObjectType.String when fieldType == typeof(char) => string.IsNullOrEmpty(String) ? default : Convert.ToChar(String),
                DocumentObjectType.String => String,
                DocumentObjectType.List when fieldType == null => List,
                DocumentObjectType.List => fieldInfo.DocumentInfo.Create(_value),
                DocumentObjectType.Dictionary when fieldType == null => Dictionary,
                DocumentObjectType.Dictionary => fieldInfo.DocumentInfo.Create(_value),
                DocumentObjectType.Timestapm when fieldType == typeof(DateTime) => Timestamp.ToDateTime(),
                DocumentObjectType.Timestapm when fieldType == typeof(DateTimeOffset) => Timestamp.ToDateTimeOffset(),
                DocumentObjectType.Timestapm => Timestamp,
                DocumentObjectType.Bytes when fieldType == typeof(byte[]) => Bytes,
                DocumentObjectType.Bytes => new MemoryStream(Bytes),
                DocumentObjectType.GeoPoint => GeoPoint,
                DocumentObjectType.DocumentReference => DocumentReference,
                _ => throw new InvalidOperationException()
            };
        }
    }
}
