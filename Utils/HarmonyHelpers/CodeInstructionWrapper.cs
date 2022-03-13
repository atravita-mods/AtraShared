﻿#if TRANSPILERS

using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace AtraShared.Utils.HarmonyHelper;

/// <summary>
/// Special cases for code instructions to match against.
/// </summary>
public enum SpecialCodeInstructionCases
{
    /// <summary>
    /// WildCard matches all codes.
    /// </summary>
    Wildcard,

    /// <summary>
    /// Matches all codes that load from an argument.
    /// </summary>
    LdArg,

    /// <summary>
    /// Matches all codes that store to an argument.
    /// </summary>
    StArg,

    /// <summary>
    /// Matches all codes that load the address of an argument.
    /// </summary>
    LdArgA,

    /// <summary>
    /// Matches all codes that load from a local.
    /// </summary>
    LdLoc,

    /// <summary>
    /// Matches all codes that store to a local.
    /// </summary>
    StLoc,
}

/// <summary>
/// Wraps the code instruction class of Harmony to allow for looser comparisons.
/// </summary>
public class CodeInstructionWrapper
{
    private readonly LocalVariableInfo? local;
    private readonly Type? localType;
    private readonly int? argumentPos;
    private readonly SpecialCodeInstructionCases? specialInstructionCase;

    private readonly CodeInstruction? codeInstruction;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeInstructionWrapper"/> class
    /// to wrap this specific <see cref="OpCode"/> operand pair.
    /// </summary>
    /// <param name="opcode">Opcode.</param>
    /// <param name="operand">Operand. Use null to match any operand.</param>
    public CodeInstructionWrapper(OpCode opcode, object? operand = null)
        => this.codeInstruction = new CodeInstruction(opcode, operand);

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeInstructionWrapper"/> class
    /// to wrap this specific <see cref="CodeInstruction"/>.
    /// </summary>
    /// <param name="instrution">instruction to wrap.</param>
    /// <remarks>A null operand matches any operand.</remarks>
    public CodeInstructionWrapper(CodeInstruction instrution)
        => this.codeInstruction = instrution;

    public CodeInstructionWrapper(SpecialCodeInstructionCases specialcase)
        => this.specialInstructionCase = specialcase;

    public CodeInstructionWrapper(SpecialCodeInstructionCases specialcase, int argument)
    {
        if (specialcase is SpecialCodeInstructionCases.LdArg or SpecialCodeInstructionCases.StArg)
        {
            this.specialInstructionCase = specialcase;
            this.argumentPos = argument;
        }
        throw new ArgumentException("Argument position can only be used with LdArg or StArg");
    }

    public CodeInstructionWrapper(SpecialCodeInstructionCases specialcase, LocalVariableInfo local)
    {
        if (specialcase is SpecialCodeInstructionCases.LdLoc or SpecialCodeInstructionCases.StLoc)
        {
            this.specialInstructionCase = specialcase;
            this.local = local;
        }
        throw new ArgumentException("Localbuilders can only be used with LdLoc or StLoc");
    }

    public CodeInstructionWrapper(SpecialCodeInstructionCases specialcase, Type localType)
    {
        if (specialcase is SpecialCodeInstructionCases.LdLoc or SpecialCodeInstructionCases.StLoc)
        {
            this.specialInstructionCase = specialcase;
            this.localType = localType;
        }
        throw new ArgumentException("Matching by type can only be used with LdLoc or StLoc");
    }

    /// <summary>
    /// Whether or not this CodeInstructionWrapper is a valid match to the code instruction.
    /// </summary>
    /// <param name="instruction">Instruction to check against.</param>
    /// <returns>True for a match.</returns>
    /// <exception cref="UnexpectedEnumValueException{SpecialCodeInstructionCases}">Recieved an unexpeced enum value.</exception>
    public bool Matches(CodeInstruction instruction)
    {
        if (this.specialInstructionCase is null)
        {
            return this.codeInstruction is not null &&
                ((this.codeInstruction.operand is null && this.codeInstruction.opcode == instruction.opcode)
                  || this.codeInstruction.Is(instruction.opcode, instruction.operand));
        }
        return this.specialInstructionCase switch
        {
            SpecialCodeInstructionCases.Wildcard => true,
            SpecialCodeInstructionCases.LdArg => this.argumentPos is null ? instruction.IsLdarg() : instruction.IsLdarg(this.argumentPos),
            SpecialCodeInstructionCases.StArg => this.argumentPos is null ? instruction.IsStarg() : instruction.IsStarg(this.argumentPos),
            SpecialCodeInstructionCases.LdArgA => this.argumentPos is null ? instruction.IsLdarga() : instruction.IsLdarga(this.argumentPos),
            SpecialCodeInstructionCases.LdLoc => this.local is null
                                                    ? (instruction.IsLdloc() && (this.localType is null || LocalBuilderOperandIsOfType(this.localType, instruction.operand)))
                                                    : (instruction.IsLdloc() && IsMatchingLocal(this.local, instruction.operand)),
            SpecialCodeInstructionCases.StLoc => this.local is null
                                                    ? (instruction.IsStloc() && (this.localType is null || LocalBuilderOperandIsOfType(this.localType, instruction.operand)))
                                                    : (instruction.IsStloc() && IsMatchingLocal(this.local, instruction.operand)),
            _ => throw new UnexpectedEnumValueException<SpecialCodeInstructionCases>(this.specialInstructionCase.Value),
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (this.specialInstructionCase is null)
        {
            if (this.codeInstruction is null)
            {
                return "null CodeInstructionWrapper";
            }
            else
            {
                return this.codeInstruction.ToString();
            }
        }
        else
        {
            return this.specialInstructionCase.Value.ToString();
        }
    }

    private static bool IsMatchingLocal(LocalVariableInfo loc, object other)
        => other is LocalVariableInfo otherloc && loc.LocalIndex == otherloc.LocalIndex && loc.LocalType == otherloc.LocalType;

    private static bool LocalBuilderOperandIsOfType(Type type, object other)
        => other is LocalVariableInfo otherloc && otherloc.LocalType == type;
}

#endif