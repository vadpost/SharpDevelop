﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System.Collections;
using ICSharpCode.NRefactory.Parser.CSharp;

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	public sealed class CSharpOutputFormatter : AbstractOutputFormatter
	{
		PrettyPrintOptions prettyPrintOptions;
		
		bool          emitSemicolon  = true;
		
		public bool EmitSemicolon {
			get {
				return emitSemicolon;
			}
			set {
				emitSemicolon = value;
			}
		}
		
		public CSharpOutputFormatter(PrettyPrintOptions prettyPrintOptions) : base(prettyPrintOptions)
		{
			this.prettyPrintOptions = prettyPrintOptions;
		}
		
		public override void PrintToken(int token)
		{
			if (token == Tokens.Semicolon && !EmitSemicolon) {
				return;
			}
			PrintText(Tokens.GetTokenString(token));
		}
		
		Stack braceStack = new Stack();
		
		public void BeginBrace(BraceStyle style, bool indent)
		{
			switch (style) {
				case BraceStyle.EndOfLine:
					if (!LastCharacterIsWhiteSpace) {
						Space();
					}
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					if (indent)
						++IndentationLevel;
					break;
				case BraceStyle.NextLine:
					NewLine();
					Indent();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					if (indent)
						++IndentationLevel;
					break;
				case BraceStyle.NextLineShifted:
					NewLine();
					if (indent)
						++IndentationLevel;
					Indent();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					break;
				case BraceStyle.NextLineShifted2:
					NewLine();
					if (indent)
						++IndentationLevel;
					Indent();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					++IndentationLevel;
					break;
			}
			braceStack.Push(style);
		}
		
		public void EndBrace(bool indent)
		{
			BraceStyle style = (BraceStyle)braceStack.Pop();
			switch (style) {
				case BraceStyle.EndOfLine:
				case BraceStyle.NextLine:
					if (indent)
						--IndentationLevel;
					Indent();
					PrintToken(Tokens.CloseCurlyBrace);
					NewLine();
					break;
				case BraceStyle.NextLineShifted:
					Indent();
					PrintToken(Tokens.CloseCurlyBrace);
					NewLine();
					if (indent)
						--IndentationLevel;
					break;
				case BraceStyle.NextLineShifted2:
					if (indent)
						--IndentationLevel;
					Indent();
					PrintToken(Tokens.CloseCurlyBrace);
					NewLine();
					--IndentationLevel;
					break;
			}
		}
		
		public override void PrintIdentifier(string identifier)
		{
			if (Keywords.GetToken(identifier) >= 0)
				PrintText("@");
			PrintText(identifier);
		}
		
		public override void PrintComment(Comment comment, bool forceWriteInPreviousBlock)
		{
			switch (comment.CommentType) {
				case CommentType.Block:
					if (forceWriteInPreviousBlock) {
						WriteInPreviousLine("/*" + comment.CommentText + "*/", forceWriteInPreviousBlock);
					} else {
						PrintSpecialText("/*" + comment.CommentText + "*/");
					}
					break;
				case CommentType.Documentation:
					WriteLineInPreviousLine("///" + comment.CommentText, forceWriteInPreviousBlock);
					break;
				default:
					WriteLineInPreviousLine("//" + comment.CommentText, forceWriteInPreviousBlock);
					break;
			}
		}
	}
}
