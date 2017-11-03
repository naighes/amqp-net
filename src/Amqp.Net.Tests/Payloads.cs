﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Amqp.Net.Client.Entities;
using Amqp.Net.Client.Frames;
using Amqp.Net.Client.Payloads;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Moq;
using Xunit;

namespace Amqp.Net.Tests
{
    public class Payloads
    {
        [Fact]
        public void EncodingAndDecodingAsWell()
        {
            Task.WhenAll(AssertFrame(new ChannelCloseFrame(0, new ChannelClosePayload(1, "text", 1, 2))),
                         AssertFrame(new ChannelCloseOkFrame(1, new ChannelCloseOkPayload())),
                         AssertFrame(new ChannelOpenFrame(2, new ChannelOpenPayload())),
                         AssertFrame(new ChannelOpenOkFrame(3, new ChannelOpenOkPayload())),
                         AssertFrame(new ConnectionCloseFrame(4, new ConnectionClosePayload(4, "some reply", 40, 30))),
                         AssertFrame(new ConnectionCloseOkFrame(5, new ConnectionCloseOkPayload())),
                         AssertFrame(new ConnectionOpenFrame(6, new ConnectionOpenPayload("/"))),
                         AssertFrame(new ConnectionOpenOkFrame(7, new ConnectionOpenOkPayload())),
                         AssertFrame(new ConnectionStartFrame(8, new ConnectionStartPayload(AmqpVersion.New(0, 9),
                                                                                            new Table(new Dictionary<String, Object> { { "11", 12 } }),
                                                                                            new[] { "PLAIN" },
                                                                                            new[] { "en_US" }))),
                         AssertFrame(new ConnectionStartOkFrame(9, new ConnectionStartOkPayload(new Table(new Dictionary<String, Object> { { "11", 12 } }),
                                                                                                "PLAIN",
                                                                                                "response",
                                                                                                "en_US"))),
                         AssertFrame(new ConnectionTuneFrame(10, new ConnectionTunePayload(4, 5, 6))),
                         AssertFrame(new ConnectionTuneOkFrame(11, new ConnectionTuneOkPayload(4, 5, 6))),
                         AssertFrame(new ExchangeBindFrame(12, new ExchangeBindPayload(0,
                                                                                       "destination",
                                                                                       "source",
                                                                                       "key",
                                                                                       false,
                                                                                       new Table(new Dictionary<String, Object> { { "11", 12 } })))),
                         AssertFrame(new ExchangeBindOkFrame(13, new ExchangeBindOkPayload())),
                         AssertFrame(new ExchangeDeclareFrame(14, new ExchangeDeclarePayload(0,
                                                                                             "name",
                                                                                             ExchangeType.Direct,
                                                                                             false,
                                                                                             true,
                                                                                             true,
                                                                                             false,
                                                                                             false,
                                                                                             new Table(new Dictionary<String, Object> { { "11", 12 } })))),
                         AssertFrame(new ExchangeDeclareOkFrame(15, new ExchangeDeclareOkPayload())),
                         AssertFrame(new ExchangeDeleteFrame(16, new ExchangeDeletePayload(0, "name", false, false))),
                         AssertFrame(new ExchangeDeleteOkFrame(17, new ExchangeDeleteOkPayload())),
                         AssertFrame(new ExchangeUnbindFrame(18, new ExchangeUnbindPayload(0,
                                                                                           "destination",
                                                                                           "source",
                                                                                           "key",
                                                                                           false,
                                                                                           new Table(new Dictionary<String, Object> { { "11", 12 } })))),
                         AssertFrame(new ExchangeUnbindOkFrame(19, new ExchangeUnbindOkPayload())),
                         AssertFrame(new QueueBindFrame(20, new QueueBindPayload(0,
                                                                                 "queueName",
                                                                                 "exchangeName",
                                                                                 "key",
                                                                                 false,
                                                                                 new Table(new Dictionary<String, Object> { { "11", 12 } })))),
                         AssertFrame(new QueueBindOkFrame(21, new QueueBindOkPayload())),
                         AssertFrame(new QueueDeclareFrame(22, new QueueDeclarePayload(0,
                                                                                       "name",
                                                                                       false,
                                                                                       true,
                                                                                       true,
                                                                                       false,
                                                                                       false,
                                                                                       new Table(new Dictionary<String, Object> { { "11", 12 } })))),
                         AssertFrame(new QueueDeclareOkFrame(23, new QueueDeclareOkPayload("name", 4, 2))),
                         AssertFrame(new QueueDeleteFrame(24, new QueueDeletePayload(0, "name", false, false, false))),
                         AssertFrame(new QueueDeleteOkFrame(25, new QueueDeleteOkPayload(9))),
                         AssertFrame(new QueueUnbindFrame(26, new QueueUnbindPayload(0,
                                                                                     "queueName",
                                                                                     "exchangeName",
                                                                                     "key",
                                                                                     new Table(new Dictionary<String, Object> { { "11", 12 } })))),
                         AssertFrame(new QueueUnbindOkFrame(27, new QueueUnbindOkPayload())),
                         AssertFrame(new BasicQosFrame(28, new BasicQosPayload(3, 4, false))),
                         AssertFrame(new BasicQosOkFrame(29, new BasicQosOkPayload())),
                         AssertFrame(new BasicConsumeFrame(30, new BasicConsumePayload(0,
                                                                                       "queueName",
                                                                                       "consumer-tag-1",
                                                                                       false,
                                                                                       true,
                                                                                       false,
                                                                                       false,
                                                                                       new Table(new Dictionary<String, Object> { { "11", 12 } })))),
                         AssertFrame(new BasicConsumeOkFrame(31, new BasicConsumeOkPayload("consumer-tag-1"))),
                         AssertFrame(new BasicDeliverFrame(32,
                                                           new BasicDeliverPayload("consumer-tag-1",
                                                                                   32L,
                                                                                   false,
                                                                                   "exchange_name",
                                                                                   "key"),
                                                           new IFrame[] {})),
                         AssertFrame(new HeaderFrame(32,
                                                     new HeaderPayload(33,
                                                                       100,
                                                                       5L,
                                                                       new ConsumedMessageProperties("text/plain",
                                                                                                     "utf-8",
                                                                                                     new Table(new Dictionary<String, Object> { { "11", 12 } }),
                                                                                                     1,
                                                                                                     0,
                                                                                                     null,
                                                                                                     null,
                                                                                                     null,
                                                                                                     "message-id",
                                                                                                     123L,
                                                                                                     "message-type",
                                                                                                     null,
                                                                                                     null,
                                                                                                     null)))),
                         AssertFrame(new BodyFrame(33, new BodyPayload(new Byte[] { }))))
                .Wait();
        }

        private static async Task AssertFrame<TPayload, TContext>(Frame<TPayload, TContext> expected)
            where TPayload : class, IFramePayload
            where TContext : IFrameContext
        {
            var channel = new FakeChannel();
            var buffer = await (expected.WriteToAsync(channel) as Task<IByteBuffer>);
            var actual = new FrameParser().Parse(buffer) as Frame<TPayload, TContext>;
            Assert.Equal(expected,
                         actual,
                         new FrameEqualityComparer<TPayload, TContext>(new FramePayloadEqualityComparer<TPayload>()));
        }

        private class FrameEqualityComparer<TPayload, TContext> : IEqualityComparer<Frame<TPayload, TContext>>
            where TPayload : class, IFramePayload
            where TContext : IFrameContext
        {
            private readonly IEqualityComparer<TPayload> comparer;

            public FrameEqualityComparer(IEqualityComparer<TPayload> comparer)
            {
                this.comparer = comparer;
            }

            public Boolean Equals(Frame<TPayload, TContext> x, Frame<TPayload, TContext> y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                return Equals(x.Header, y.Header) && comparer.Equals(x.Payload, y.Payload);
            }

            public Int32 GetHashCode(Frame<TPayload, TContext> instance)
            {
                unchecked
                {
                    if (instance == null)
                        return 0;

                    var result = 0;
                    result = (result * 397) ^ instance.Header.GetHashCode();
                    result = (result * 397) ^ comparer.GetHashCode(instance.Payload);

                    return result;
                }
            }
        }

        private class FramePayloadEqualityComparer<T> : IEqualityComparer<T>
            where T : IFramePayload
        {
            private static IEnumerable<MemberInfo> Members(Object obj)
            {
                const BindingFlags flags = BindingFlags.Instance |
                                           BindingFlags.NonPublic |
                                           BindingFlags.Public;

                return obj.GetType()
                          .GetFields(flags)
                          .Cast<MemberInfo>()
                          .Union(obj.GetType()
                                    .GetProperties(flags));
            }

            public Int32 GetHashCode(T instance)
            {
                unchecked
                {
                    if (instance == null)
                        return 0;

                    var members = Members(instance);
                    var result = 0;

                    foreach (var member in members)
                    {
                        Object value;

                        if (member is FieldInfo info)
                            value = info.GetValue(instance);
                        else
                            value = ((PropertyInfo)member).GetValue(instance);

                        switch (value) {
                            case null:
                                continue;
                            case Array array:
                                result = (result * 397) ^ new ArrayEqualityComparer().GetHashCode(array);
                                break;
                            default:
                                result = (result * 397) ^ value.GetHashCode();
                                break;
                        }
                    }

                    return result;
                }
            }

            public Boolean Equals(T x, T y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                var members = Members(x);
                var result = true;

                foreach (var member in members)
                {
                    Object v1;
                    Object v2;

                    if (member is FieldInfo info)
                    {
                        v1 = info.GetValue(x);
                        v2 = info.GetValue(y);
                    }
                    else
                    {
                        v1 = ((PropertyInfo)member).GetValue(x);
                        v2 = ((PropertyInfo)member).GetValue(y);
                    }

                    if (v1 is Array && v2 is Array)
                        result = result && new ArrayEqualityComparer().Equals(v1 as Array, v2 as Array);
                    else
                        result = result && Equals(v1, v2);
                }

                return result;
            }
        }

        private class ArrayEqualityComparer : IEqualityComparer<Array>
        {
            public Boolean Equals(Array x, Array y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                if (x.Length != y.Length)
                    return false;

                for (var i = 0; i < x.Length; i++)
                    if (!Equals(x.GetValue(i), y.GetValue(i)))
                        return false;

                return true;
            }

            public Int32 GetHashCode(Array instance)
            {
                unchecked
                {
                    if (instance == null)
                        return 0;

                    var result = 0;

                    for (var i = 0; i < instance.Length; i++)
                        result = (result * 397) ^ (instance.GetValue(i)?.GetHashCode() ?? 0);

                    return result;
                }
            }
        }

        #region fake channel

        private class FakeChannel : DotNetty.Transport.Channels.IChannel
        {
            public IAttribute<T> GetAttribute<T>(AttributeKey<T> key)
                where T : class
            {
                throw new NotImplementedException();
            }

            public Boolean HasAttribute<T>(AttributeKey<T> key)
                where T : class
            {
                throw new NotImplementedException();
            }

            public Int32 CompareTo(DotNetty.Transport.Channels.IChannel other)
            {
                throw new NotImplementedException();
            }

            public Task DeregisterAsync()
            {
                throw new NotImplementedException();
            }

            public Task BindAsync(EndPoint localAddress)
            {
                throw new NotImplementedException();
            }

            public Task ConnectAsync(EndPoint remoteAddress)
            {
                throw new NotImplementedException();
            }

            public Task ConnectAsync(EndPoint remoteAddress, EndPoint localAddress)
            {
                throw new NotImplementedException();
            }

            public Task DisconnectAsync()
            {
                throw new NotImplementedException();
            }

            public Task CloseAsync()
            {
                throw new NotImplementedException();
            }

            public DotNetty.Transport.Channels.IChannel Read()
            {
                throw new NotImplementedException();
            }

            public Task WriteAsync(Object message)
            {
                throw new NotImplementedException();
            }

            public DotNetty.Transport.Channels.IChannel Flush()
            {
                throw new NotImplementedException();
            }

            public Task WriteAndFlushAsync(Object message)
            {
                var buffer = message as IByteBuffer;
                return Task.FromResult(buffer);
            }

            public IChannelId Id { get; }

            public IByteBufferAllocator Allocator
            {
                get
                {
                    var allocator = new Mock<IByteBufferAllocator>();
                    allocator.Setup(_ => _.Buffer()).Returns(Unpooled.Buffer());

                    return allocator.Object;
                }
            }

            public IEventLoop EventLoop { get; }

            public DotNetty.Transport.Channels.IChannel Parent { get; }

            public Boolean Open { get; }

            public Boolean Active { get; }

            public Boolean Registered { get; }

            public ChannelMetadata Metadata { get; }

            public EndPoint LocalAddress { get; }

            public EndPoint RemoteAddress { get; }

            public Boolean IsWritable { get; }

            public IChannelUnsafe Unsafe { get; }

            public IChannelPipeline Pipeline { get; }

            public IChannelConfiguration Configuration { get; }

            public Task CloseCompletion { get; }
        }

        #endregion
    }
}