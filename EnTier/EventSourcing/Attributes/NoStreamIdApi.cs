using System;
using System.Net.Http;

namespace EnTier.EventSourcing.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class NoStreamIdApi : Attribute
    {

    }
}