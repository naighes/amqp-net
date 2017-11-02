﻿using System;
using System.Collections.Generic;
using Amqp.Net.Client.Decoding;
using Amqp.Net.Client.Payloads;
using DotNetty.Buffers;

namespace Amqp.Net.Client.Frames
{
    internal struct MethodFrameDescriptor
    {
        internal readonly Int16 ClassId;
        internal readonly Int16 MethodId;

        private static readonly IDictionary<MethodFrameDescriptor, Func<FrameHeader, IByteBuffer, IFrame>> map =
            new Dictionary<MethodFrameDescriptor, Func<FrameHeader, IByteBuffer, IFrame>>
                {
                    // connection class
                    { ConnectionStartPayload.StaticDescriptor, (header, buffer) => new ConnectionStartFrame(header.ChannelIndex,
                                                                                                            ConnectionStartPayload.Parse(buffer)) },
                    { ConnectionStartOkPayload.StaticDescriptor, (header, buffer) => new ConnectionStartOkFrame(header.ChannelIndex,
                                                                                                                ConnectionStartOkPayload.Parse(buffer)) },
                    { ConnectionTunePayload.StaticDescriptor, (header, buffer) => new ConnectionTuneFrame(header.ChannelIndex,
                                                                                                          ConnectionTunePayload.Parse(buffer)) },
                    { ConnectionTuneOkPayload.StaticDescriptor, (header, buffer) => new ConnectionTuneOkFrame(header.ChannelIndex,
                                                                                                              ConnectionTuneOkPayload.Parse(buffer)) },
                    { ConnectionOpenPayload.StaticDescriptor, (header, buffer) => new ConnectionOpenFrame(header.ChannelIndex,
                                                                                                          ConnectionOpenPayload.Parse(buffer)) },
                    { ConnectionOpenOkPayload.StaticDescriptor, (header, buffer) => new ConnectionOpenOkFrame(header.ChannelIndex,
                                                                                                              ConnectionOpenOkPayload.Parse(buffer)) },
                    { ConnectionClosePayload.StaticDescriptor, (header, buffer) => new ConnectionCloseFrame(header.ChannelIndex,
                                                                                                            ConnectionClosePayload.Parse(buffer)) },
                    { ConnectionCloseOkPayload.StaticDescriptor, (header, buffer) => new ConnectionCloseOkFrame(header.ChannelIndex,
                                                                                                                ConnectionCloseOkPayload.Parse(buffer)) },
                    // channel class
                    { ChannelOpenPayload.StaticDescriptor, (header, buffer) => new ChannelOpenFrame(header.ChannelIndex,
                                                                                                    ChannelOpenPayload.Parse(buffer)) },
                    { ChannelOpenOkPayload.StaticDescriptor, (header, buffer) => new ChannelOpenOkFrame(header.ChannelIndex,
                                                                                                        ChannelOpenOkPayload.Parse(buffer)) },
                    { ChannelClosePayload.StaticDescriptor, (header, buffer) => new ChannelCloseFrame(header.ChannelIndex,
                                                                                                      ChannelClosePayload.Parse(buffer)) },
                    { ChannelCloseOkPayload.StaticDescriptor, (header, buffer) => new ChannelCloseOkFrame(header.ChannelIndex,
                                                                                                          ChannelCloseOkPayload.Parse(buffer)) },
                    // exchange class
                    { ExchangeDeclarePayload.StaticDescriptor, (header, buffer) => new ExchangeDeclareFrame(header.ChannelIndex,
                                                                                                            ExchangeDeclarePayload.Parse(buffer)) },
                    { ExchangeDeclareOkPayload.StaticDescriptor, (header, buffer) => new ExchangeDeclareOkFrame(header.ChannelIndex,
                                                                                                                ExchangeDeclareOkPayload.Parse(buffer)) },
                    { ExchangeBindPayload.StaticDescriptor, (header, buffer) => new ExchangeBindFrame(header.ChannelIndex,
                                                                                                      ExchangeBindPayload.Parse(buffer)) },
                    { ExchangeBindOkPayload.StaticDescriptor, (header, buffer) => new ExchangeBindOkFrame(header.ChannelIndex,
                                                                                                          ExchangeBindOkPayload.Parse(buffer)) },
                    { ExchangeUnbindPayload.StaticDescriptor, (header, buffer) => new ExchangeUnbindFrame(header.ChannelIndex,
                                                                                                          ExchangeUnbindPayload.Parse(buffer)) },
                    { ExchangeUnbindOkPayload.StaticDescriptor, (header, buffer) => new ExchangeUnbindOkFrame(header.ChannelIndex,
                                                                                                              ExchangeUnbindOkPayload.Parse(buffer)) },
                    { ExchangeDeletePayload.StaticDescriptor, (header, buffer) => new ExchangeDeleteFrame(header.ChannelIndex,
                                                                                                          ExchangeDeletePayload.Parse(buffer)) },
                    { ExchangeDeleteOkPayload.StaticDescriptor, (header, buffer) => new ExchangeDeleteOkFrame(header.ChannelIndex,
                                                                                                              ExchangeDeleteOkPayload.Parse(buffer)) },
                    // queue class
                    { QueueDeclarePayload.StaticDescriptor, (header, buffer) => new QueueDeclareFrame(header.ChannelIndex,
                                                                                                      QueueDeclarePayload.Parse(buffer)) },
                    { QueueDeclareOkPayload.StaticDescriptor, (header, buffer) => new QueueDeclareOkFrame(header.ChannelIndex,
                                                                                                          QueueDeclareOkPayload.Parse(buffer)) },
                    { QueueBindPayload.StaticDescriptor, (header, buffer) => new QueueBindFrame(header.ChannelIndex,
                                                                                                QueueBindPayload.Parse(buffer)) },
                    { QueueBindOkPayload.StaticDescriptor, (header, buffer) => new QueueBindOkFrame(header.ChannelIndex,
                                                                                                    QueueBindOkPayload.Parse(buffer)) },
                    { QueueUnbindPayload.StaticDescriptor, (header, buffer) => new QueueUnbindFrame(header.ChannelIndex,
                                                                                                    QueueUnbindPayload.Parse(buffer)) },
                    { QueueUnbindOkPayload.StaticDescriptor, (header, buffer) => new QueueUnbindOkFrame(header.ChannelIndex,
                                                                                                        QueueUnbindOkPayload.Parse(buffer)) },
                    { QueueDeletePayload.StaticDescriptor, (header, buffer) => new QueueDeleteFrame(header.ChannelIndex,
                                                                                                    QueueDeletePayload.Parse(buffer)) },
                    { QueueDeleteOkPayload.StaticDescriptor, (header, buffer) => new QueueDeleteOkFrame(header.ChannelIndex,
                                                                                                        QueueDeleteOkPayload.Parse(buffer)) },
                    // basic class
                    { BasicQosPayload.StaticDescriptor, (header, buffer) => new BasicQosFrame(header.ChannelIndex,
                                                                                              BasicQosPayload.Parse(buffer)) },
                    { BasicQosOkPayload.StaticDescriptor, (header, buffer) => new BasicQosOkFrame(header.ChannelIndex,
                                                                                                  BasicQosOkPayload.Parse(buffer)) },
                    { BasicConsumePayload.StaticDescriptor, (header, buffer) => new BasicConsumeFrame(header.ChannelIndex,
                                                                                                      BasicConsumePayload.Parse(buffer)) },
                    { BasicConsumeOkPayload.StaticDescriptor, (header, buffer) => new BasicConsumeOkFrame(header.ChannelIndex,
                                                                                                          BasicConsumeOkPayload.Parse(buffer)) },
                    { BasicDeliverPayload.StaticDescriptor, (header, buffer) => new BasicDeliverFrame(header.ChannelIndex,
                                                                                                      BasicDeliverPayload.Parse(buffer)) }
                };

        internal IFrame BuildFrame(FrameHeader header, IByteBuffer buffer)
        {
            return map[this](header, buffer);
        }

        internal static MethodFrameDescriptor Parse(IByteBuffer buffer)
        {
            return new MethodFrameDescriptor(buffer.ReadShort(), buffer.ReadShort());
        }

        internal MethodFrameDescriptor(Int16 classId, Int16 methodId)
        {
            ClassId = classId;
            MethodId = methodId;
        }

        internal void Write(IByteBuffer buffer)
        {
            Int16FieldValueCodec.Instance.Encode(ClassId, buffer);
            Int16FieldValueCodec.Instance.Encode(MethodId, buffer);
        }

        public Boolean Equals(MethodFrameDescriptor other)
        {
            return ClassId == other.ClassId && MethodId == other.MethodId;
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is MethodFrameDescriptor descriptor && Equals(descriptor);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (ClassId.GetHashCode() * 397) ^ MethodId.GetHashCode();
            }
        }

        public static Boolean operator ==(MethodFrameDescriptor left, MethodFrameDescriptor right)
        {
            return left.Equals(right);
        }

        public static Boolean operator !=(MethodFrameDescriptor left, MethodFrameDescriptor right)
        {
            return !left.Equals(right);
        }

        public override String ToString()
        {
            return $"{{\"class_id\":{ClassId},\"method_id\":{MethodId}}}";
        }
    }
}