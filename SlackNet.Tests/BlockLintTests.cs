using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyAssertions;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace SlackNet.Tests;

public class BlockLintTests
{
    [Test]
    public void AllBlockElementsHaveACorrespondingElementValue()
    {
        var allBlockElementTypes = ExportedConcreteTypesAssignableTo<IBlockElement>();
        var allElementValueTypes = ExportedConcreteTypesAssignableTo<ElementValue>();

        allBlockElementTypes
            .Except(ExcludedBlockElements)
            .Where(t => !allElementValueTypes.Contains(t))
            .ShouldBeEmpty();
    }

    static readonly HashSet<string> ExcludedBlockElements =
        [
            "button",
            "image"
        ];

    [Test]
    public void AllTypesWithActionIdHaveACorrespondingBlockAction()
    {
        var allTypesWithActionId = typeof(BlockAction).Assembly.ExportedTypes
            .Where(t =>
                !t.IsAssignableTo(typeof(BlockAction)) // BlockActions themselves are excluded
                && t != typeof(BlockOptionsRequest) // BlockOptionsRequest is sent from Slack
                && t != typeof(EntityActionButton) // These just trigger a ButtonAction
                && t.GetProperty("ActionId") != null
                && !t.IsAbstract)
            .Select(t => t.GetTypeInfo().SlackType())
            .ToHashSet();

        var allBlockActionTypes = ExportedConcreteTypesAssignableTo<BlockAction>();

        allTypesWithActionId
            .Except(ExcludedActionElements)
            .Where(t => !allBlockActionTypes.Contains(t))
            .ShouldBeEmpty();
    }

    private static readonly HashSet<string> ExcludedActionElements =
        [
            "file_input"
        ];

    [Test]
    public void AllContextActionElementsHa() { }

    static HashSet<string> ExportedConcreteTypesAssignableTo<T>()
    {
        return typeof(T).Assembly.ExportedTypes
            .Where(t =>
                t.IsAssignableTo(typeof(T))
                && !t.IsAbstract)
            .Select(t => t.GetTypeInfo().SlackType())
            .ToHashSet();
    }
}