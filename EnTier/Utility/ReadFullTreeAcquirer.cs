using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection;
using EnTier.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EnTier.Utility;

public interface IReadFullTreeAcquirer
{
    bool IsReadByIdFullTree { get; }
    bool IsReadAllFullTree { get; }
}

internal class ReadFullTreeAcquirer : IReadFullTreeAcquirer
{
    private readonly bool _readAllInControllerLevel;
    private readonly bool _readByIdInControllerLevel;


    public ReadFullTreeAcquirer(ControllerBase controller) : this(controller.GetType())
    {
    }

    public ReadFullTreeAcquirer(Type controllerType)
    {
        var controllerAttribute =
            controllerType.GetCustomAttribute<FullTreeReadAttribute>() ?? new FullTreeReadAttribute(false, false);

        _readAllInControllerLevel = controllerAttribute.ReadAll;

        _readByIdInControllerLevel = controllerAttribute.ReadById;
    }

    private bool IsActionFullTreeRead<T>(Expression<Func<FullTreeReadAttribute, T>> selectExpression)
    {
        var member = MemberOwnerUtilities.GetKey(selectExpression).Headless().ToString().ToLower();
        var askedForById = nameof(FullTreeReadAttribute.ReadById).ToLower() == member;
        var askedForAll = nameof(FullTreeReadAttribute.ReadAll).ToLower() == member;

        var stack = new StackTrace();

        var frames = stack.GetFrames() ?? new StackFrame[] { };

        foreach (var frame in frames)
        {
            if (frame != null)
            {
                var method = frame.GetMethod();

                if (method != null)
                {
                    var attribute = method.GetCustomAttribute<FullTreeActionAttribute>() ;

                    if (attribute != null)
                    {
                        if (askedForById)
                        {
                            return attribute.ReadById;
                        }

                        if (askedForAll)
                        {
                            return attribute.ReadAll;
                        }

                        return false;    
                    }
                }
            }
        }

        if (askedForById)
        {
            return _readByIdInControllerLevel;
        }

        if (askedForAll)
        {
            return _readAllInControllerLevel;
        }

        return false;
    }

    public bool IsReadByIdFullTree => IsActionFullTreeRead(a => a.ReadById);
    public bool IsReadAllFullTree => IsActionFullTreeRead(a => a.ReadAll);
}