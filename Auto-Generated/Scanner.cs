// Generated by TinyPG v1.3 available at www.codeproject.com

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Configurator
{
	#region Scanner

	public partial class Scanner
	{
		public string Input { get; set; }
		public int StartPosition { get; set; }
		public int EndPosition { get; set; }
		public string CurrentFile { get; set; }
		public int CurrentLine { get; set; }
		public int CurrentColumn { get; set; }
		public int CurrentPosition { get; set; }
		/// <summary>
		/// tokens that were skipped
		/// </summary>
		public List<Token> Skipped { get; set; }
		public Dictionary<TokenType, Regex> Patterns { get; set; }

		private Token LookAheadToken;
		private List<TokenType> Tokens;
		private List<TokenType> SkipList; // tokens to be skipped


		public Scanner()
		{
			StartPosition = 0;
			EndPosition = 0;
			Regex regex;
			Patterns = new Dictionary<TokenType, Regex>();
			Tokens = new List<TokenType>();
			LookAheadToken = null;
			Skipped = new List<Token>();

			SkipList = new List<TokenType>();
			SkipList.Add(TokenType.WHITESPACE);
			SkipList.Add(TokenType.COMMENT);

			regex = new Regex(@"^$", RegexOptions.Compiled);
			Patterns.Add(TokenType.EOF, regex);
			Tokens.Add(TokenType.EOF);

			regex = new Regex(@"\r?\n\s*|(?=#.*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.ENDLINE, regex);
			Tokens.Add(TokenType.ENDLINE);

			regex = new Regex(@"=(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.EQUAL, regex);
			Tokens.Add(TokenType.EQUAL);

			regex = new Regex(@"<(?!/)(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.TAGOPEN, regex);
			Tokens.Add(TokenType.TAGOPEN);

			regex = new Regex(@"</(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.TAGOPENALT, regex);
			Tokens.Add(TokenType.TAGOPENALT);

			regex = new Regex(@"<:(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.NSOPEN, regex);
			Tokens.Add(TokenType.NSOPEN);

			regex = new Regex(@"</:(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.NSCLOSE, regex);
			Tokens.Add(TokenType.NSCLOSE);

			regex = new Regex(@"<@(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.MULTILINETAGOPEN, regex);
			Tokens.Add(TokenType.MULTILINETAGOPEN);

			regex = new Regex(@"</@(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.MULTILINETAGOPENALT, regex);
			Tokens.Add(TokenType.MULTILINETAGOPENALT);

			regex = new Regex(@"<\*(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.LISTOPEN, regex);
			Tokens.Add(TokenType.LISTOPEN);

			regex = new Regex(@"</\*(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.LISTOPENALT, regex);
			Tokens.Add(TokenType.LISTOPENALT);

			regex = new Regex(@">", RegexOptions.Compiled);
			Patterns.Add(TokenType.TAGCLOSE, regex);
			Tokens.Add(TokenType.TAGCLOSE);

			regex = new Regex(@"[A-Za-z_][A-Za-z0-9_]*(?!\s*\r?\n\s*)", RegexOptions.Compiled);
			Patterns.Add(TokenType.NAME, regex);
			Tokens.Add(TokenType.NAME);

			regex = new Regex(@"(?s).*?(?=</@)", RegexOptions.Compiled);
			Patterns.Add(TokenType.MULTILINECONTENT, regex);
			Tokens.Add(TokenType.MULTILINECONTENT);

			regex = new Regex(@"[^""#\s][^#\r\n]*", RegexOptions.Compiled);
			Patterns.Add(TokenType.SINGLELINECONTENT, regex);
			Tokens.Add(TokenType.SINGLELINECONTENT);

			regex = new Regex(@"""([^\r\n""]|"""")*""", RegexOptions.Compiled);
			Patterns.Add(TokenType.QUOTEDCONTENT, regex);
			Tokens.Add(TokenType.QUOTEDCONTENT);

			regex = new Regex(@"(?!</\*)(?!<@)[^\s""#]((?!</\*)[^\s#])*", RegexOptions.Compiled);
			Patterns.Add(TokenType.SIMPLEITEM, regex);
			Tokens.Add(TokenType.SIMPLEITEM);

			regex = new Regex(@"(?!</\*)(?!<@)""([^\r\n""]|"""")*""", RegexOptions.Compiled);
			Patterns.Add(TokenType.QUOTEDITEM, regex);
			Tokens.Add(TokenType.QUOTEDITEM);

			regex = new Regex(@"\s+", RegexOptions.Compiled);
			Patterns.Add(TokenType.WHITESPACE, regex);
			Tokens.Add(TokenType.WHITESPACE);

			regex = new Regex(@"#.*(?=\n|$)", RegexOptions.Compiled);
			Patterns.Add(TokenType.COMMENT, regex);
			Tokens.Add(TokenType.COMMENT);


		}

		public void Init(string input)
		{
			Init(input, "");
		}

		public void Init(string input, string fileName)
		{
			this.Input = input;
			StartPosition = 0;
			EndPosition = 0;
			CurrentFile = fileName;
			CurrentLine = 1;
			CurrentColumn = 1;
			CurrentPosition = 0;
			LookAheadToken = null;
		}

		public Token GetToken(TokenType type)
		{
			Token t = new Token(this.StartPosition, this.EndPosition);
			t.Type = type;
			return t;
		}

		/// <summary>
		/// executes a lookahead of the next token
		/// and will advance the scan on the input string
		/// </summary>
		/// <returns></returns>
		public Token Scan(params TokenType[] expectedtokens)
		{
			Token tok = LookAhead(expectedtokens); // temporarely retrieve the lookahead
			LookAheadToken = null; // reset lookahead token, so scanning will continue
			StartPosition = tok.EndPosition;
			EndPosition = tok.EndPosition; // set the tokenizer to the new scan position
			CurrentLine = tok.Line + (tok.Text.Length - tok.Text.Replace("\n", "").Length);
			CurrentFile = tok.File;
			return tok;
		}

		/// <summary>
		/// returns token with longest best match
		/// </summary>
		/// <returns></returns>
		public Token LookAhead(params TokenType[] expectedtokens)
		{
			int i;
			int startpos = StartPosition;
			int endpos = EndPosition;
			int currentline = CurrentLine;
			string currentFile = CurrentFile;
			Token tok = null;
			List<TokenType> scantokens;


			// this prevents double scanning and matching
			// increased performance
			if (LookAheadToken != null
				&& LookAheadToken.Type != TokenType._UNDETERMINED_
				&& LookAheadToken.Type != TokenType._NONE_) return LookAheadToken;

			// if no scantokens specified, then scan for all of them (= backward compatible)
			if (expectedtokens.Length == 0)
				scantokens = Tokens;
			else
			{
				scantokens = new List<TokenType>(expectedtokens);
				scantokens.AddRange(SkipList);
			}

			do
			{

				int len = -1;
				TokenType index = (TokenType)int.MaxValue;
				string input = Input.Substring(startpos);

				tok = new Token(startpos, endpos);

				for (i = 0; i < scantokens.Count; i++)
				{
					Regex r = Patterns[scantokens[i]];
					Match m = r.Match(input);
					if (m.Success && m.Index == 0 && ((m.Length > len) || (scantokens[i] < index && m.Length == len)))
					{
						len = m.Length;
						index = scantokens[i];
					}
				}

				if (index >= 0 && len >= 0)
				{
					tok.EndPosition = startpos + len;
					tok.Text = Input.Substring(tok.StartPosition, len);
					tok.Type = index;
				}
				else if (tok.StartPosition == tok.EndPosition)
				{
					if (tok.StartPosition < Input.Length)
						tok.Text = Input.Substring(tok.StartPosition, 1);
					else
						tok.Text = "EOF";
				}

				// Update the line and column count for error reporting.
				tok.File = currentFile;
				tok.Line = currentline;
				if (tok.StartPosition < Input.Length)
					tok.Column = tok.StartPosition - Input.LastIndexOf('\n', tok.StartPosition);

				if (SkipList.Contains(tok.Type))
				{
					startpos = tok.EndPosition;
					endpos = tok.EndPosition;
					currentline = tok.Line + (tok.Text.Length - tok.Text.Replace("\n", "").Length);
					currentFile = tok.File;
					Skipped.Add(tok);
				}
				else
				{
					// only assign to non-skipped tokens
					tok.Skipped = Skipped; // assign prior skips to this token
					Skipped = new List<Token>(); //reset skips
				}

			}
			while (SkipList.Contains(tok.Type));

			LookAheadToken = tok;
			return tok;
		}
	}

	#endregion

	#region Token

	public enum TokenType
	{

		//Non terminal tokens:
		_NONE_ = 0,
		_UNDETERMINED_ = 1,

		//Non terminal tokens:
		Start = 2,
		Namespace = 3,
		Declaration = 4,
		SimpleDeclaration = 5,
		ComplexDeclaration = 6,
		MultiLineDeclaration = 7,
		MultiLineItem = 8,
		ListDeclaration = 9,
		NamespaceBegin = 10,
		NamespaceEnd = 11,
		TagBegin = 12,
		TagEnd = 13,
		MultiLineTagBegin = 14,
		MultiLineTagEnd = 15,
		MultiLineListItemBegin = 16,
		MultiLineListItemEnd = 17,
		ListTagBegin = 18,
		ListTagEnd = 19,

		//Terminal tokens:
		EOF = 20,
		ENDLINE = 21,
		EQUAL = 22,
		TAGOPEN = 23,
		TAGOPENALT = 24,
		NSOPEN = 25,
		NSCLOSE = 26,
		MULTILINETAGOPEN = 27,
		MULTILINETAGOPENALT = 28,
		LISTOPEN = 29,
		LISTOPENALT = 30,
		TAGCLOSE = 31,
		NAME = 32,
		MULTILINECONTENT = 33,
		SINGLELINECONTENT = 34,
		QUOTEDCONTENT = 35,
		SIMPLEITEM = 36,
		QUOTEDITEM = 37,
		WHITESPACE = 38,
		COMMENT = 39
	}

	public class Token
	{
		public string File { get; set; }
		public int Line { get; set; }
		public int Column { get; set; }
		public int StartPosition { get; set; }
		public int EndPosition { get; set; }
		public string Text { get; set; }
		public object Value { get; set; }

		/// <summary>
		///  contains all prior skipped symbols
		/// </summary>
		public List<Token> Skipped { get; set; }

		public int Length { get { return EndPosition - StartPosition; } }

		[XmlAttribute]
		public TokenType Type;

		public Token()
			: this(0, 0)
		{
		}

		public Token(int start, int end)
		{
			Type = TokenType._UNDETERMINED_;
			StartPosition = start;
			EndPosition = end;
			Text = ""; // must initialize with empty string, may cause null reference exceptions otherwise
			Value = null;
		}

		public void UpdateRange(Token token)
		{
			if (token.StartPosition < this.StartPosition) this.StartPosition = token.StartPosition;
			if (token.EndPosition > this.EndPosition) this.EndPosition = token.EndPosition;
		}

		public override string ToString()
		{
			if (Text != null)
				return Type.ToString() + " '" + Text + "'";
			else
				return Type.ToString();
		}
	}

	#endregion
}
