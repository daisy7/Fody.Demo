using Daisy7.Fody.Demo.Attribute;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daisy7.Fody.Demo.Fody
{
    public class ModuleWeaver
    {
        public ModuleDefinition ModuleDefinition { get; set; }
        public void Execute()
        {

            var types = this.ModuleDefinition.Types.Where(n => n.CustomAttributes.Any(y => y.AttributeType.Resolve().Name == "WeaveType"));

            foreach (var type in types)
            {
                foreach (var method in type.Methods)
                {
                    var attrs = method.CustomAttributes.Where(y => y.AttributeType.Resolve().BaseType.Name == "WeaveMethod");
                    foreach (var attr in attrs)
                    {
                        //var resolve = attr.AttributeType.Resolve();
                        var ilProcessor = method.Body.GetILProcessor();
                        var firstInstruction = ilProcessor.Body.Instructions.First();
                        //var onActionBefore = resolve.GetMethods().Single(n => n.Name == "OnActionBefore");
                        var Reference = this.ModuleDefinition.Import(typeof(Log).GetMethod("OnActionBefore"));
                        var mfReference = this.ModuleDefinition.Import(typeof(System.Reflection.MethodBase).GetMethod("GetCurrentMethod"));
                        ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Call, mfReference));

                        MakeArrayOfArguments(method, firstInstruction, ilProcessor, 0, method.Parameters.Count, this.ModuleDefinition);
                        ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Call, Reference));
                    }
                }
            }
        }
        /// <summary>
        /// 取消编译
        /// </summary>
        public void Cancel()
        {

        }
        /// <summary>
        /// Weaving 完成之后
        /// </summary>
        public void AfterWeaving()
        {

        }
        /// <summary>
        /// 构建函数参数
        /// </summary>
        /// <param name="method">要注入的方法</param>
        /// <param name="firstInstruction">函数体内第一行指令认 IL_0000: nop</param>
        /// <param name="writer">mono IL处理容器</param>
        /// <param name="firstArgument">默认第0个参数开始</param>
        /// <param name="argumentCount">函数参数的数量，静态数据可以拿到</param>
        /// <param name="assembly">要注入的程序集</param>
        public static void MakeArrayOfArguments(MethodDefinition method, Instruction firstInstruction, ILProcessor writer, int firstArgument,
                                          int argumentCount, ModuleDefinition module)
        {
            //实例函数第一个参数值为this(当前实例对象),所以要从1开始。
            int thisShift = method.IsStatic ? 0 : 1;

            if (argumentCount > 0)
            {
                //我们先创建个和原函数参数，等长的空数组。
                writer.InsertBefore(firstInstruction, writer.Create(OpCodes.Ldc_I4, argumentCount - firstArgument));
                //然后实例object数组，赋值给我们创建的数组
                writer.InsertBefore(firstInstruction, writer.Create(OpCodes.Newarr,
                                           module.Import(typeof(object))));

                //c#代码描述
                //object[] arr=new object[argumentCount - firstArgument] 
                for (int i = firstArgument; i < argumentCount; i++)  //遍历参数
                {
                    var parameter = method.Parameters[i];

                    //在堆栈上复制一个值
                    writer.InsertBefore(firstInstruction, writer.Create(OpCodes.Dup));
                    //将常量 i - firstArgument 进行压栈，数组[i - firstArgument] 这个东东。
                    writer.InsertBefore(firstInstruction, writer.Create(OpCodes.Ldc_I4, i - firstArgument));
                    //将第i + thisShift个参数 压栈。  
                    writer.InsertBefore(firstInstruction, writer.Create(OpCodes.Ldarg, (short)(i + thisShift)));
                    //装箱成object
                    ToObject(module, firstInstruction, parameter.ParameterType, writer);
                    //压栈给数组 arr[i]赋值
                    writer.InsertBefore(firstInstruction, writer.Create(OpCodes.Stelem_Ref));

                    //c#代码描述
                    // arr[i]=value;
                }
            }
            else
            {
                writer.InsertBefore(firstInstruction, writer.Create(OpCodes.Ldnull));
            }
        }
        public static void ToObject(ModuleDefinition module, Instruction firstInstruction, TypeReference originalType, ILProcessor writer)
        {
            if (originalType.IsValueType)
            {
                //普通值类型进行装箱操作
                writer.InsertBefore(firstInstruction, writer.Create(OpCodes.Box, originalType));
            }
            else
            {
                if (originalType.IsGenericParameter)
                {
                    //集合装箱
                    writer.InsertBefore(firstInstruction, writer.Create(OpCodes.Box, module.Import(originalType)));
                }

            }
        }
    }
}
