﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Transport.Channels
{
    using DotNetty.Common.Utilities;

    public abstract class SimpleChannelInboundHandler2<I> : ChannelHandlerAdapter
        where I : class
    {
        readonly bool _autoRelease;

        protected SimpleChannelInboundHandler2() : this(true)
        {
        }

        protected SimpleChannelInboundHandler2(bool autoRelease)
        {
            _autoRelease = autoRelease;
        }

        public virtual bool TryAcceptInboundMessage(object msg, out I imsg)
        {
            imsg = msg as I;
            return imsg is object;
        }

        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            bool release = true;
            try
            {
                if (TryAcceptInboundMessage(msg, out I imsg))
                {
                    ChannelRead0(ctx, imsg);
                }
                else
                {
                    release = false;
                    _ = ctx.FireChannelRead(msg);
                }
            }
            finally
            {
                if (_autoRelease && release)
                {
                    _ = ReferenceCountUtil.Release(msg);
                }
            }
        }

        protected abstract void ChannelRead0(IChannelHandlerContext ctx, I msg);
    }
}
