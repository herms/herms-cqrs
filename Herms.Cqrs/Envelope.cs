using System;

namespace Herms.Cqrs
{
    public class Envelope<T> where T : Command {}
}