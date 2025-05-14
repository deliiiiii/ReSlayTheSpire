// using Microsoft.CodeAnalysis.Text;
// using Microsoft.CodeAnalysis;
// using System.Text;
// using Microsoft.CodeAnalysis.CSharp;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
// using System.Collections.Generic;
// using System.Linq;
//
// namespace SubstanceP
// {
//     [Generator]
//     internal class TransitionSourceGenerator : ISourceGenerator
//     {
//         public void Initialize(GeneratorInitializationContext ctx) =>
//             ctx.RegisterForSyntaxNotifications(() => new TransitionSyntaxReceiver());
//
//         public void Execute(GeneratorExecutionContext ctx)
//         {
//             if (ctx.SyntaxReceiver is not TransitionSyntaxReceiver receiver) return;
//
//             StringBuilder partialBuilder = new(), extensionBuilder = new(), ctorBuilder = new(), initBuilder = new(), processBuilder = new();
//
//             foreach (var cls in receiver.Transitions)
//             {
//                 string className = cls.Identifier.Text, namesp = cls.GetNamespace();
//                 var targetType = cls.GetAttribute("TransitionTarget").GetUnique<TypeOfExpressionSyntax>().Type;
//                 var starting = namesp == string.Empty ? string.Empty : $"namespace {namesp}\n{{";
//
//                 partialBuilder.Clear();
//                 extensionBuilder.Clear();
//                 ctorBuilder.Clear();
//                 initBuilder.Clear();
//                 processBuilder.Clear();
//
//                 foreach (var us in cls.GetUsingsInFile())
//                 {
//                     partialBuilder.AppendLine($"using {us};");
//                     extensionBuilder.AppendLine($"using {us};");
//                 }
//
//                 partialBuilder.Append($@"
// {starting}
//     public partial class {className} : Transition
//     {{
//         private readonly {targetType} target;
//                 ");
//
//                 extensionBuilder.Append($@"
// {starting}
//     public static class {targetType}ExtensionsForTransition
//     {{
//                 ");
//
//                 ctorBuilder.Append($@"
//         public {className}({targetType} target) : base()
//         {{
//             this.target = target;
//                 ").AppendLine();
//
//                 initBuilder.Append($@"
//         protected override void Init()
//         {{
//             base.Init();
//                 ").AppendLine();
//
//                 processBuilder.Append($@"
//         protected override void Process(float delta)
//         {{
//             base.Process(delta);
//
//             if (!target)
//             {{
//                 Drop();
//                 return;
//             }}
//                 ").AppendLine();
//
//                 foreach (var field in receiver.Fields)
//                 {
//                     if (field.GetClass() != className) continue;
//
//                     var fieldType = field.Declaration.Type;
//                     var fieldName = field.Declaration.Variables.First().Identifier.Text;
//                     var fieldNameNew = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
//                     var relative = field.GetAttribute("TransitionField").GetUnique<LiteralExpressionSyntax>().Token.ValueText;
//
//                     partialBuilder.Append($@"
//         private {fieldType} from{fieldNameNew}, to{fieldNameNew};
//         private partial void Init{fieldNameNew}({targetType} target);
//         private partial void Process{fieldNameNew}({targetType} target, {fieldType} delta);
//         public {className} From{fieldNameNew}({fieldType} {fieldName})
//         {{
//             this.{fieldName} = from{fieldNameNew} = {fieldName};
//             return this;
//         }}
//         public {className} To{fieldNameNew}({fieldType} {fieldName})
//         {{
//             to{fieldNameNew} = {fieldName};
//             return this;
//         }}
//                     ");
//
//                     extensionBuilder.Append($@"
//         public static {className} From{fieldNameNew}(this {targetType} target, {fieldType} {fieldName}) => new {className}(target).From{fieldNameNew}({fieldName});
//         public static {className} To{fieldNameNew}(this {targetType} target, {fieldType} {fieldName}) => new {className}(target).From{fieldNameNew}(target.{relative}).To{fieldNameNew}({fieldName});
//         public static {className} Add{fieldNameNew}(this {targetType} target, {fieldType} {fieldName}) => new {className}(target).From{fieldNameNew}(target.{relative}).To{fieldNameNew}(target.{relative} + {fieldName});
//                     ");
//
//                     ctorBuilder.AppendLine($"\t\t\tthis.{fieldName} = from{fieldNameNew} = to{fieldNameNew} = target.{relative};");
//                     initBuilder.AppendLine($"\t\t\tInit{fieldNameNew}(target);");
//                     processBuilder.AppendLine($"\t\t\tProcess{fieldNameNew}(target, delta * (to{fieldNameNew} - from{fieldNameNew}));");
//                 }
//
//                 ctorBuilder.AppendLine("\t\t}");
//                 partialBuilder.Append(ctorBuilder.ToString());
//                 initBuilder.AppendLine("\t\t}");
//                 partialBuilder.Append(initBuilder.ToString());
//                 processBuilder.AppendLine("\t\t}");
//                 partialBuilder.Append(processBuilder.ToString());
//
//                 var ending = "\n\t}" + (namesp == string.Empty ? string.Empty : "\n}");
//
//                 partialBuilder.Append(ending);
//                 extensionBuilder.Append(ending);
//
//                 ctx.AddSource($"{className}.g.cs", SourceText.From(partialBuilder.ToString(), Encoding.UTF8));
//                 ctx.AddSource($"{targetType}ExtensionsForTransition.g.cs", SourceText.From(extensionBuilder.ToString(), Encoding.UTF8));
//             }
//         }
//     }
//
//     internal class TransitionSyntaxReceiver : ISyntaxReceiver
//     {
//         internal List<ClassDeclarationSyntax> Transitions { get; } = new();
//         internal List<FieldDeclarationSyntax> Fields { get; } = new();
//         public void OnVisitSyntaxNode(SyntaxNode node)
//         {
//             if (node is ClassDeclarationSyntax classNode &&
//                 classNode.HasAttribute("TransitionTarget") &&
//                 classNode.IsPartialClass() &&
//                 classNode.DirectlyInheritsFrom("Transition"))
//                 Transitions.Add(classNode);
//
//             if (node is FieldDeclarationSyntax fieldNode &&
//                 fieldNode.HasAttribute("TransitionField"))
//                 Fields.Add(fieldNode);
//         }
//     }
//
//     internal static class SyntaxExtensions
//     {
//         internal static bool DirectlyInheritsFrom(this ClassDeclarationSyntax node, string name)
//             => node.BaseList != null && node.BaseList.Types.Any(t => t.ToString() == name);
//
//         internal static bool IsPartialClass(this ClassDeclarationSyntax node)
//             => node.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
//
//         internal static IEnumerable<string> GetUsingsInFile(this ClassDeclarationSyntax node)
//             => node.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().Select(u => u.Name.ToString());
//
//         internal static string GetNamespace(this SyntaxNode node)
//         {
//             var current = node;
//             while (current is not NamespaceDeclarationSyntax)
//             {
//                 current = current.Parent;
//                 if (current == null) return string.Empty;
//             }
//             return (current as NamespaceDeclarationSyntax).Name.ToString();
//         }
//
//         internal static bool HasAttribute(this MemberDeclarationSyntax node, string name)
//             => node.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == name));
//
//         internal static AttributeSyntax GetAttribute(this MemberDeclarationSyntax node, string name)
//         {
//             foreach (var al in node.AttributeLists)
//                 foreach (var a in al.Attributes)
//                     if (a.Name.ToString() == name) return a;
//             return null;
//         }
//
//         internal static T GetUnique<T>(this AttributeSyntax node) where T : ExpressionSyntax
//             => node.ArgumentList.Arguments.Select(a => a.Expression).OfType<T>().First();
//
//         internal static string GetClass(this FieldDeclarationSyntax node)
//             => (node.Parent as ClassDeclarationSyntax).Identifier.Text;
//     }
// }