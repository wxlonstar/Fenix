﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Codecs.Http.WebSockets
{
    using DotNetty.Buffers;
    using DotNetty.Handlers.Streams;

    public sealed class WebSocketChunkedInput : IChunkedInput<WebSocketFrame>
    {
        readonly IChunkedInput<IByteBuffer> input;
        readonly int rsv;

        public WebSocketChunkedInput(IChunkedInput<IByteBuffer> input)
            : this(input, 0)
        {
        }

        public WebSocketChunkedInput(IChunkedInput<IByteBuffer> input, int rsv)
        {
            if (input is null) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.input); }

            this.input = input;
            this.rsv = rsv;
        }

        public bool IsEndOfInput => this.input.IsEndOfInput;

        public void Close() => this.input.Close();

        public WebSocketFrame ReadChunk(IByteBufferAllocator allocator)
        {
            IByteBuffer buf = this.input.ReadChunk(allocator);
            return buf is object ? new ContinuationWebSocketFrame(this.input.IsEndOfInput, this.rsv, buf) : null;
        }

        public long Length => this.input.Length;

        public long Progress => this.input.Progress;
    }
}
