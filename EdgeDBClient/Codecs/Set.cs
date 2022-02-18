﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeDB.Codecs
{
    public class Set<TInner> : ICodec<Models.DataTypes.Set<TInner?>>
    {
        private readonly ICodec<TInner> _innerCodec;

        public Set(ICodec<TInner> innerCodec)
        {
            _innerCodec = innerCodec;
        }

        public Models.DataTypes.Set<TInner?>? Deserialize(PacketReader reader)
        {
            if (_innerCodec is Array<TInner>)
                return DecodeSetOfArrays(reader);
            else return DecodeSet(reader);
        }

        public void Serialize(PacketWriter writer, Models.DataTypes.Set<TInner?>? value)
        {
            throw new NotImplementedException();
        }

        private Models.DataTypes.Set<TInner?>? DecodeSetOfArrays(PacketReader reader)
        {
            var dimensions = reader.ReadInt32();

            // discard flags and reserved
            reader.ReadBytes(8);

            if(dimensions == 0)
            {
                return new Models.DataTypes.Set<TInner?>();
            }

            if(dimensions != 1)
            {
                throw new NotSupportedException("Only dimensions of 1 are supported for arrays");
            }

            var upper = reader.ReadInt32();
            var lower = reader.ReadInt32();

            var numElements = upper - lower + 1;

            var result = new TInner?[numElements];

            for(int i = 0; i != numElements; i++)
            {
                reader.ReadBytes(4); // skip array element size

                var envelopeElements = reader.ReadInt32();

                if(envelopeElements != 1)
                {
                    throw new InvalidDataException($"Envelope should contain 1 element, got {envelopeElements} elements");
                }

                reader.ReadBytes(4); // skip reserved

                result[i] = _innerCodec.Deserialize(reader);
            }

            return new Models.DataTypes.Set<TInner?>(result);
        }

        private Models.DataTypes.Set<TInner?>? DecodeSet(PacketReader reader)
        {
            var dimensions = reader.ReadInt32();

            // discard flags and reserved
            reader.ReadBytes(8);

            if (dimensions == 0)
            {
                return new Models.DataTypes.Set<TInner?>();
            }

            if (dimensions != 1)
            {
                throw new NotSupportedException("Only dimensions of 1 are supported for sets");
            }

            var upper = reader.ReadInt32();
            var lower = reader.ReadInt32();

            var numElements = upper - lower + 1;

            var result = new TInner?[numElements];

            for(int i = 0; i != numElements; i++)
            {
                var elementLength = reader.ReadInt32();

                if (elementLength == -1)
                    result[i] = default; // TODO: better 'null' value handling?
                else
                    result[i] = _innerCodec.Deserialize(reader);
            }

            return new Models.DataTypes.Set<TInner?>(result);
        }
    }
}
