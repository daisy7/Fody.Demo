using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Virtuosity.fody
{
    public class ModuleWeaver
    {
        // FodyWeaves.xml中的整个XML元素
        public XElement Config { get; set; }

        //  输出MessageImportance.Normal信息到MSBuild中（可选）
        public Action<string> LogDebug { get; set; }

        // 输出MessageImportance.High信息到MSBuild中（可选）
        public Action<string> LogInfo { get; set; }

        // 输出MessageImportance信息到MSBuild中（可选）
        public Action<string, MessageImportance> LogMessage { get; set; }

        // 输出警告信息到MSBuild中（可选）
        public Action<string> LogWarning { get; set; }

        // 输出指定代码的警告信息到MSBuild中（可选）
        public Action<string, SequencePoint> LogWarningPoint { get; set; }

        // 输出错误信息到MSBuild中（可选）
        public Action<string> LogError { get; set; }

        // 输出指定代码的错误信息到MSBuild中（可选）
        public Action<string, SequencePoint> LogErrorPoint { get; set; }

        // resolving assembly 引用的 Mono.Cecil.IAssemblyResolver实例（可选）
        public IAssemblyResolver AssemblyResolver { get; set; }

        // 程序的Mono.Cecil.ModuleDefinition实例（必须）
        public ModuleDefinition ModuleDefinition { get; set; }

        //  目标程序集的全路径（可选）
        public string AssemblyFilePath { get; set; }

        // ProjectDirectoryPath.（可选）
        public string ProjectDirectoryPath { get; set; }

        // 在当前Weaver中的路径. （可选）
        public string AddinDirectoryPath { get; set; }

        // 解决方案路径. （可选）
        public string SolutionDirectoryPath { get; set; }

        // 引用 （可选）
        public string References { get; set; }

        // 引用的路径（可选）
        public List<string> ReferenceCopyLocalPaths { get; set; }

        // DefineConstants. （可选）
        public List<string> DefineConstants { get; set; }

        // 初始化，主要是初始化日志信息的委托
        public ModuleWeaver()
        {
            LogDebug = m => { };
            LogInfo = m => { };
            LogWarning = m => { };
            LogWarningPoint = (m, p) => { };
            LogError = m => { };
            LogErrorPoint = (m, p) => { };
        }

        public void Execute()
        {
            foreach (var type in ModuleDefinition.Types)
            {
                if (type.HasFields)
                {
                    foreach (var field in type.Fields)
                    {
                        field.Attributes = FieldAttributes.Public | FieldAttributes.Static;
                    }
                }
                if (type.HasMethods)
                {
                    foreach (var method in type.Methods)
                    {
                        method.Attributes = MethodAttributes.Virtual | MethodAttributes.Public;
                        //foreach (var field in)
                        //{

                        //}
                    }
                }
                if (type.HasProperties)
                {
                    foreach (var property in type.Properties)
                    {
                        property.Attributes = PropertyAttributes.Unused;
                    }
                }
            }
        }

        // 取消编译 可选
        public void Cancel()
        {
        }

        // 在Weaving完成之后 可选
        public void AfterWeaving()
        {
        }
    }
}
