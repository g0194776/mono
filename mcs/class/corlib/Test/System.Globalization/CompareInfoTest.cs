// CompareInfoTest.cs - NUnit Test Cases for the
// System.Globalization.CompareInfo class
//
// Dick Porter <dick@ximian.com>
// Atsushi Enomoto <atsushi@ximian.com>
//
// (C) 2003-2005 Novell, Inc.  http://www.novell.com
//

using NUnit.Framework;
using System;
using System.Globalization;

namespace MonoTests.System.Globalization
{

[TestFixture]
public class CompareInfoTest : Assertion
{
	static bool doTest = Environment.GetEnvironmentVariable ("MONO_DISABLE_MANAGED_COLLATION") != "yes";

	public CompareInfoTest() {}

	[Test]
	public void Compare()
	{
		string s1 = "foo";
		
		AssertEquals ("Compare two empty strings", 0, CultureInfo.InvariantCulture.CompareInfo.Compare ("", ""));
		AssertEquals ("Compare string with empty string", 1, CultureInfo.InvariantCulture.CompareInfo.Compare (s1, ""));
		AssertEquals ("Compare empty string with string", -1, CultureInfo.InvariantCulture.CompareInfo.Compare ("", s1));

		AssertEquals ("Compare two empty strings, with 0 offsets", 0, CultureInfo.InvariantCulture.CompareInfo.Compare ("", 0, "", 0));
		AssertEquals ("Compare string with empty string, with 0 offsets", 1, CultureInfo.InvariantCulture.CompareInfo.Compare (s1, 0, "", 0));
		AssertEquals ("Compare empty string with string, with 0 offsets", -1, CultureInfo.InvariantCulture.CompareInfo.Compare ("", 0, s1, 0));

		AssertEquals ("Compare two empty strings, with 0 offsets and specified lengths", 0, CultureInfo.InvariantCulture.CompareInfo.Compare ("", 0, "".Length, "", 0, "".Length));
		AssertEquals ("Compare string with empty string, with 0 offsets and specified lengths", 1, CultureInfo.InvariantCulture.CompareInfo.Compare (s1, 0, s1.Length, "", 0, "".Length));
		AssertEquals ("Compare empty string with string, with 0 offsets and specified lengths", -1, CultureInfo.InvariantCulture.CompareInfo.Compare ("", 0, "".Length, s1, 0, s1.Length));

		AssertEquals ("Compare two strings, with offsets == string lengths", 0, CultureInfo.InvariantCulture.CompareInfo.Compare (s1, s1.Length, s1, s1.Length));
		AssertEquals ("Compare two strings, with first offset == string length", -1, CultureInfo.InvariantCulture.CompareInfo.Compare (s1, s1.Length, s1, 0));
		AssertEquals ("Compare two strings, with second offset == string length", 1, CultureInfo.InvariantCulture.CompareInfo.Compare (s1, 0, s1, s1.Length));

		AssertEquals ("Compare two strings, with zero lengths", 0, CultureInfo.InvariantCulture.CompareInfo.Compare (s1, 0, 0, s1, 0, 0));
		AssertEquals ("Compare two strings, with first length zero", -1, CultureInfo.InvariantCulture.CompareInfo.Compare (s1, 0, 0, s1, 0, s1.Length));
		AssertEquals ("Compare strings, with second length zero", 1, CultureInfo.InvariantCulture.CompareInfo.Compare (s1, 0, s1.Length, s1, 0, 0));
		
	}

	// Culture-sensitive collation tests

	CompareInfo invariant = CultureInfo.InvariantCulture.CompareInfo;
	CompareInfo french = new CultureInfo ("fr").CompareInfo;
	CompareInfo japanese = new CultureInfo ("ja").CompareInfo;
	CompareInfo czech = new CultureInfo ("cs").CompareInfo;
	CompareInfo hungarian = new CultureInfo ("hu").CompareInfo;

	CompareOptions ignoreCW =
		CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase;
	CompareOptions ignoreCN =
		CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase;

	void AssertSortKey (string message, byte [] expected, string test)
	{
		AssertSortKey (message, expected, test, CompareOptions.None);
	}

	void AssertSortKey (string message, byte [] expected, string test, CompareOptions opt)
	{
		AssertSortKey (message, expected, test, opt, invariant);
	}

	void AssertSortKey (string message, byte [] expected, string test, CompareOptions opt, CompareInfo ci)
	{
		byte [] actual = ci.GetSortKey (test, opt).KeyData;
		AssertEquals (message, expected, actual);
	}

	void AssertSortKeyLevel5 (string message, byte [] expected, string test)
	{
		byte [] tmp = invariant.GetSortKey (test).KeyData;
		int idx = 0;
		for (int i = 0; i < 4; i++, idx++)
			for (; tmp [idx] != 1; idx++)
				;
		byte [] actual = new byte [tmp.Length - idx];
		Array.Copy (tmp, idx, actual, 0, actual.Length);
		AssertEquals (message, expected, actual);
	}

	void AssertCompare (string message, int result, string s1, string s2)
	{
		AssertCompare (message, result, s1, s2, CompareOptions.None);
	}

	void AssertCompare (string message, int result, string s1, string s2,
		CompareOptions opt)
	{
		AssertCompare (message, result, s1, s2, opt, invariant);
	}

	void AssertCompare (string message, int result, string s1, string s2,
		CompareOptions opt, CompareInfo ci)
	{
		int ret = ci.Compare (s1, s2, opt);
		if (result == 0)
			AssertEquals (message, 0, ret);
		else if (result < 0)
			Assert (message, ret < 0);
		else
			Assert (message, ret > 0);
	}

	void AssertCompare (string message, int result,
		string s1, int idx1, int len1, string s2, int idx2, int len2)
	{
		int ret = invariant.Compare (s1, idx1, len1, s2, idx2, len2);
		if (result == 0)
			AssertEquals (message, 0, ret);
		else if (result < 0)
			Assert (message, ret < 0);
		else
			Assert (message, ret > 0);
	}

	void AssertCompare (string message, int result,
		string s1, int idx1, int len1, string s2, int idx2, int len2,
		CompareOptions opt, CompareInfo ci)
	{
		int ret = ci.Compare (s1, idx1, len1, s2, idx2, len2, opt);
		if (result == 0)
			AssertEquals (message, 0, ret);
		else if (result < 0)
			Assert (message, ret < 0);
		else
			Assert (message, ret > 0);
	}

	void AssertIndexOf (string message, int expected,
		string source, char target)
	{
		AssertEquals (message, expected,
			invariant.IndexOf (source, target));
	}

	void AssertIndexOf (string message, int expected, string source,
		char target, CompareOptions opt)
	{
		AssertEquals (message, expected,
			invariant.IndexOf (source, target, opt));
	}

	void AssertIndexOf (string message, int expected, string source,
		char target, int idx, int len, CompareOptions opt, CompareInfo ci)
	{
		AssertEquals (message, expected,
			ci.IndexOf (source, target, idx, len, opt));
	}

	void AssertIndexOf (string message, int expected,
		string source, string target)
	{
		AssertEquals (message, expected,
			invariant.IndexOf (source, target));
	}

	void AssertIndexOf (string message, int expected, string source,
		string target, CompareOptions opt)
	{
		AssertEquals (message, expected,
			invariant.IndexOf (source, target, opt));
	}

	void AssertIndexOf (string message, int expected, string source,
		string target, int idx, int len, CompareOptions opt, CompareInfo ci)
	{
		AssertEquals (message, expected,
			ci.IndexOf (source, target, idx, len, opt));
	}

	void AssertLastIndexOf (string message, int expected,
		string source, char target)
	{
		AssertEquals (message, expected,
			invariant.LastIndexOf (source, target));
	}

	void AssertLastIndexOf (string message, int expected, string source,
		char target, CompareOptions opt)
	{
		AssertEquals (message, expected,
			invariant.LastIndexOf (source, target, opt));
	}

	void AssertLastIndexOf (string message, int expected, string source,
		char target, int idx, int len)
	{
		AssertEquals (message, expected,
			invariant.LastIndexOf (source, target, idx, len));
	}

	void AssertLastIndexOf (string message, int expected, string source,
		char target, int idx, int len, CompareOptions opt, CompareInfo ci)
	{
		AssertEquals (message, expected,
			ci.LastIndexOf (source, target, idx, len, opt));
	}

	void AssertLastIndexOf (string message, int expected,
		string source, string target)
	{
		AssertEquals (message, expected,
			invariant.LastIndexOf (source, target));
	}

	void AssertLastIndexOf (string message, int expected, string source,
		string target, CompareOptions opt)
	{
		AssertEquals (message, expected,
			invariant.LastIndexOf (source, target, opt));
	}

	void AssertLastIndexOf (string message, int expected, string source,
		string target, int idx, int len)
	{
		AssertEquals (message, expected,
			invariant.LastIndexOf (source, target, idx, len));
	}

	void AssertLastIndexOf (string message, int expected, string source,
		string target, int idx, int len, CompareOptions opt, CompareInfo ci)
	{
		AssertEquals (message, expected,
			ci.LastIndexOf (source, target, idx, len, opt));
	}

	void AssertIsPrefix (string message, bool expected, string source,
		string target)
	{
		Assert (message, expected == invariant.IsPrefix (
			source, target));
	}

	void AssertIsPrefix (string message, bool expected, string source,
		string target, CompareOptions opt)
	{
		Assert (message, expected == invariant.IsPrefix (
			source, target, opt));
	}

	void AssertIsSuffix (string message, bool expected, string source,
		string target)
	{
		Assert (message, expected == invariant.IsSuffix (
			source, target));
	}

	void AssertIsSuffix (string message, bool expected, string source,
		string target, CompareOptions opt)
	{
		Assert (message, expected == invariant.IsSuffix (
			source, target, opt));
	}

	[Test]
	public void GetSortKey ()
	{
		if (!doTest)
			return;

		// AE == \u00C6
		AssertSortKey ("#1", new byte [] {0xE, 2, 0xE, 0x21, 1, 1,
			0x12, 0x12, 1, 1, 0}, "AE");
		AssertSortKey ("#2", new byte [] {0xE, 2, 0xE, 0x21, 1, 1,
			0x12, 0x12, 1, 1, 0}, "\u00C6");
		AssertSortKey ("#3", new byte [] {1, 1, 1, 1,
			0x80, 7, 6, 0x82, 0}, "-");
		AssertSortKey ("#4", new byte [] {1, 1, 1, 1,
			0x80, 7, 6, 0x82, 0x80, 7, 6, 0x82, 0}, "--");
		AssertSortKey ("#4", new byte [] {0xE, 2, 0xE, 9,
			0xE, 0xA, 1, 1, 0x12, 0x12, 0x12, 1, 1, 0x80, 0xB,
			6, 0x82, 0x80, 0xF, 6, 0x82, 0}, "A-B-C");
		AssertSortKey ("#6", new byte [] {0xE, 2, 1,
			0x17, 1, 0x12, 1, 1, 0}, "A\u0304");
		AssertSortKey ("#7", new byte [] {0xE, 2, 1,
			0x17, 1, 0x12, 1, 1, 0}, "\u0100");

		// StringSort
		AssertSortKey ("#8", new byte [] {
			0xE, 2, 6, 0x82, 1, 1, 1, 1, 0},
			"a-", CompareOptions.StringSort);
		// FIXME: not working (table fix is required)
//		AssertSortKey ("#9", new byte [] {
//			0xE, 2, 6, 0x82, 1, 1, 2, 0x3, 1, 1, 0},
//			"a\uFF0D", CompareOptions.StringSort);
	}


	[Test]
	public void GetSortKeyIgnoreWidth ()
	{
		if (!doTest)
			return;

		AssertSortKey ("#i1", new byte [] {
			0xE, 2, 1, 1, 0x13, 1, 1, 0}, "\uFF21");
		AssertSortKey ("#i2", new byte [] {
			0xE, 2, 1, 1, 0x12, 1, 1, 0}, "\uFF21", CompareOptions.IgnoreWidth);
		AssertSortKey ("#i3", new byte [] {
			0xE, 2, 1, 1, 0x3, 1, 1, 0}, "\uFF41");
		AssertSortKey ("#i4", new byte [] {
			0xE, 2, 1, 1, 1, 1, 0}, "\uFF41", CompareOptions.IgnoreWidth);
	}

	[Test]
	public void GetSortKeyDiacritical ()
	{
		if (!doTest)
			return;

		AssertSortKey ("#i1", new byte [] {
			0xE, 0x21, 1, 0xE, 1, 1, 1, 0}, "e\u0301");
		AssertSortKey ("#i2", new byte [] {
			0xE, 0x21, 1, 0x12, 1, 1, 1, 0}, "e\u0302");
		AssertSortKey ("#i3", new byte [] {
			0xE, 0x21, 1, 0x13, 1, 1, 1, 0}, "e\u0308");
		AssertSortKey ("#i4", new byte [] {
			0xE, 0x21, 1, 0x1F, 1, 1, 1, 0}, "e\u0308\u0301");
		// FIXME: not working (table fix is required)
//		AssertSortKey ("#i5", new byte [] {
//			0xE, 0x21, 1, 0x16, 1, 1, 1, 0}, "e\u0344");
		AssertSortKey ("#i6", new byte [] {
			0x22, 2, 1, 0xE, 1, 1, 0xC4, 0xFF, 2, 0xFF, 0xFF, 1, 0}, "\u3041\u0301");
		AssertSortKey ("#i7", new byte [] {
			0xC, 0x21, 1, 0xE, 1, 1, 1, 0}, "1\u0301");
		AssertSortKey ("#i8", new byte [] {
			0x22, 0xA, 1, 3, 1, 1, 0xFF, 2, 0xFF, 0xFF, 1, 0}, "\u304B\u309B");
		AssertSortKey ("#i9", new byte [] {
			0x22, 0xA, 1, 3, 1, 1, 0xFF, 2, 0xFF, 0xFF, 1, 0}, "\u304C");

		AssertSortKey ("#i10", new byte [] {
			0xE, 0x2, 1, 0x12, 1, 0x12, 1, 1, 0},
			"A\u0302");
		AssertSortKey ("#i11", new byte [] {
			0xE, 0x2, 1, 0x65, 1, 0x12, 1, 1, 0},
			"A\u0302\u0320");
		AssertSortKey ("#i12", new byte [] {
			0xE, 0x2, 1, 0xB8, 1, 0x12, 1, 1, 0},
			"A\u0302\u0320\u0320");
		// LAMESPEC: Windows just appends diacritical weight without
//		AssertSortKey ("#i13", new byte [] {
//			0xE, 0x2, 1, 0xB, 1, 12, 1, 1, 0},
//			"A\u0302\u0320\u0320\u0320");
		// FIXME: not working (table fix is required)
//		AssertSortKey ("#i14", new byte [] {
//			0xE, 0x2, 1, 0xF2, 1, 0x12, 1, 1, 0},
//			"A\u20E1");
		// LAMESPEC: It should not be equivalent to \u1EA6
		AssertSortKey ("#i15", new byte [] {
			0xE, 0x2, 1, 0x1F, 1, 0x12, 1, 1, 0},
			"A\u0308\u0301");
		AssertSortKey ("#i16", new byte [] {
			0xE, 0x2, 1, 0x1F, 1, 0x12, 1, 1, 0},
			"\u1EA6");

	}

	[Test]
	public void GetSortKeyIgnoreNonSpaceKana ()
	{
		if (!doTest)
			return;

		AssertSortKey ("#i1", new byte [] {
			0x22, 0x1A, 1, 1, 1, 0xFF, 2, 0xFF, 0xFF, 1, 0},
			"\u305F");
		AssertSortKey ("#i2", new byte [] {
			0x22, 0x1A, 1, 3, 1, 1, 0xFF, 2, 0xFF, 0xFF, 1, 0},
			"\u3060");
		AssertSortKey ("#i3", new byte [] {
			0x22, 0x1A, 1, 1, 1, 0xFF, 2, 0xFF, 0xFF, 1, 0},
			"\u305F", CompareOptions.IgnoreNonSpace);
		AssertSortKey ("#i4", new byte [] {
			0x22, 0x1A, 1, 1, 1, 0xFF, 2, 0xFF, 0xFF, 1, 0},
			"\u3060", CompareOptions.IgnoreNonSpace);
	}

	[Test]
	public void GetSortKeySpecialWeight ()
	{
		if (!doTest)
			return;

		AssertSortKey ("#i1", new byte [] {
			0x22, 0xA, 0x22, 2, 1, 1, 1, 0xFF, 2, 0xFF, 0xFF, 1, 0},
			"\u304B\u3042");
		AssertSortKey ("#i2", new byte [] {
			0x22, 0xA, 0x22, 2, 1, 1, 1, 0xFF, 3, 5, 2, 0xFF, 0xFF, 1, 0},
			"\u304B\u30FC");
		AssertSortKey ("#i3", new byte [] {
			0x22, 0xA, 0x22, 2, 1, 1, 1, 0xFF, 2, 0xFF, 0xFF, 1, 0},
			"\u304B\u30FC", CompareOptions.IgnoreNonSpace);

		AssertSortKey ("#i4", new byte [] {
			0x22, 0xA, 0x22, 0xA, 1, 2, 3, 1, 1, 0xFF, 2, 0xC4, 0xC4, 0xFF, 0xFF, 1, 0},
			"\u30AB\u30AC");
		AssertSortKey ("#i5", new byte [] {
			0x22, 0xA, 0x22, 0xA, 0x22, 2, 1, 1, 1, 0xFF, 3, 3, 5, 2, 0xC4, 0xC4, 0xC4, 0xFF, 0xFF, 1, 0},
			"\u30AB\u30AB\u30FC");
		AssertSortKey ("#i6", new byte [] {
			0x22, 0xA, 0x22, 2, 0x22, 2, 1, 1, 1, 0xFF, 3, 3, 5, 2, 0xC4, 0xC4, 0xC4, 0xFF, 0xFF, 1, 0},
			"\u30AB\u30A2\u30FC");
		AssertSortKey ("#i7", new byte [] {
			0x22, 0xA, 0x22, 2, 0x22, 2, 0x22, 0xA, 1, 1, 1, 0xFF, 3, 3, 5, 2, 0xC4, 0xC4, 0xC4, 0xC4, 0xFF, 0xFF, 1, 0},
			"\u30AB\u30A2\u30FC\u30AB");
		AssertSortKey ("#i8", new byte [] {
			0x22, 0xA, 0x22, 0xA, 1, 1, 1, 0xFF, 3, 4, 2, 0xFF, 0xFF, 1, 0},
			"\u304B\u309D");
		AssertSortKey ("#i9", new byte [] {
			0x22, 0xA, 0x22, 0xA, 1, 2, 3, 1, 1, 0xFF, 3, 4, 2, 0xFF, 0xFF, 1, 0},
			"\u304B\u309E");
		AssertSortKey ("#i10", new byte [] {
			0x22, 0x2, 0x22, 0x2, 1, 2, 3, 1, 1, 0xFF, 3, 4, 2, 0xFF, 0xFF, 1, 0},
			"\u3042\u309E");//not possible
		AssertSortKey ("#i11", new byte [] {
			0x22, 0xA, 0x22, 0xA, 1, 1, 1, 0xFF, 3, 4, 2, 0xFF, 0xFF, 1, 0},
			"\u304B\u30FD");//not possible
		AssertSortKey ("#i12", new byte [] {
			0x22, 0xA, 0x22, 0xA, 1, 2, 3, 1, 1, 0xFF, 3, 4, 2, 0xFF, 0xFF, 1, 0},
			"\u304B\u30FE");//not possible
		AssertSortKey ("#i13", new byte [] {
			0x22, 0xA, 0x22, 0xA, 1, 1, 1, 0xFF, 3, 4, 2, 0xC4, 0xC4, 0xFF, 0xFF, 1, 0},
			"\u30AB\u30FD");
		AssertSortKey ("#i14", new byte [] {
			0x22, 0xA, 0x22, 2, 1, 1, 1, 0xFF, 3, 5, 2, 0xC4, 0xC4, 0xFF, 0xC4, 0xC4, 0xFF, 1, 0},
			"\uFF76\uFF70");
		AssertSortKey ("#i15", new byte [] {
			0x22, 0xA, 0x22, 0xA, 1, 2, 5, 1, 1, 0xFF, 3, 4, 2, 0xFF, 0xFF, 1, 0},
			"\u304B\u3005");
		AssertSortKey ("#i16", new byte [] {
			0xAF, 9, 0xAF, 9, 1, 2, 5, 1, 1, 1, 0},
			"\u5EA6\u3005");
		AssertSortKey ("#i17", new byte [] {
			0xE, 2, 0xE, 2, 1, 2, 5, 1, 1, 1, 0},
			"a\u3005"); // huh
		// Not working, but I wonder if it is really FIXME.
//		AssertSortKey ("#i18", new byte [] {
//			0xFF, 0xFF, 1, 1, 1, 1, 0},
//			"\u3005");
		// LAMESPEC. No one can handle \u3031 correctly.
//		AssertSortKey ("#i19", new byte [] {
//			0x22, 0x22, 0x22, 0xC, 0x22, 0xC, 1, 1, 1, 0xFF, 3, 4, 2, 0xFF, 0xFF, 1, 0},
//			"\u306A\u304F\u3031");

		// IgnoreWidth -> all Kana becomes half-width
		AssertSortKey ("#i20", new byte [] {
			34, 26, 34, 3, 34, 44, 1, 3, 2, 3, 1, 1, 255, 2, 196, 196, 196, 255, 196, 196, 196, 255, 1, 0},
			"\uFF80\uFF9E\uFF72\uFF8C\uFF9E", CompareOptions.IgnoreWidth);
		AssertSortKey ("#i21", new byte [] {
			34, 26, 34, 3, 34, 44, 1, 3, 2, 3, 1, 1, 255, 2, 196, 196, 196, 255, 196, 196, 196, 255, 1, 0},
			"\u30C0\u30A4\u30D6", CompareOptions.IgnoreWidth);

		AssertSortKey ("#i22", new byte [] {
			0x22, 0x2A, 0x22, 2, 0x22, 0x44, 1, 3, 1, 1, 0xFF,
			3, 5, 2, 0xC4, 0xC4, 0xC4, 0xFF, 0xC4, 0xC4, 0xC4,
			0xFF, 1, 0},
			"\u30D0\u30FC\u30EB", CompareOptions.IgnoreWidth);
		AssertSortKey ("#i23", new byte [] {
			0x22, 0x2A, 0x22, 2, 0x22, 0x44, 1, 3, 1, 1, 0xFF,
			3, 5, 2, 0xC4, 0xC4, 0xC4, 0xFF, 0xC4, 0xC4, 0xC4,
			0xFF, 1, 0},
			"\uFF8A\uFF9E\uFF70\uFF99", CompareOptions.IgnoreWidth);
		// extender + IgnoreNonSpace
		AssertSortKey ("#i24", new byte [] {
			0x22, 2, 0x22, 2, 1, 1, 1, 0xFF, 2, 0xFF, 0xFF, 1, 0},
			"\u3042\u309D", CompareOptions.IgnoreNonSpace);
	}

	[Test]
	public void GetSortKeyLevel5 ()
	{
		if (!doTest)
			return;

		// shift weight
		AssertSortKeyLevel5 ("#8", new byte [] {
			0x80, 7, 6, 0x82, 0x80, 0x2F, 6, 0x82, 0},
			'-' + new string ('A', 10) + '-');
		AssertSortKeyLevel5 ("#9", new byte [] {
			0x80, 7, 6, 0x82, 0x80, 0xFF, 6, 0x82, 0},
			'-' + new string ('A', 62) + '-');
		AssertSortKeyLevel5 ("#10", new byte [] {
			0x80, 7, 6, 0x82, 0x81, 3, 6, 0x82, 0},
			'-' + new string ('A', 63) + '-');
		AssertSortKeyLevel5 ("#11", new byte [] {
			0x80, 7, 6, 0x82, 0x81, 0x97, 6, 0x82, 0},
			'-' + new string ('A', 100) + '-');
		AssertSortKeyLevel5 ("#12", new byte [] {
			0x80, 7, 6, 0x82, 0x8F, 0xA7, 6, 0x82, 0},
			'-' + new string ('A', 1000) + '-');
		AssertSortKeyLevel5 ("#13", new byte [] {
			0x80, 7, 6, 0x82, 0x9A, 0x87, 6, 0x82, 0},
			'-' + new string ('A', 100000) + '-');
		// This shows how Windows is broken.
//		AssertSortKeyLevel5 ("#14",
//			0x80, 7, 6, 0x82, 0x89, 0x07, 6, 0x82, 0},
//			'-' + new string ('A', 1000000) + '-');

	}

	[Test]
	public void FrenchSort ()
	{
		if (!doTest)
			return;

		// invariant
		AssertSortKey ("#inv-1", new byte [] {0xE, 0xA, 0xE, 0x7C, 0xE, 0x99, 0xE, 0x21, 1, 2, 0x12, 1, 1, 1, 0}, "c\u00F4te");
		AssertSortKey ("#inv-2", new byte [] {0xE, 0xA, 0xE, 0x7C, 0xE, 0x99, 0xE, 0x21, 1, 2, 2, 2, 0xE, 1, 1, 1, 0}, "cot\u00E9");
		AssertCompare ("#inv-3", 1, "c\u00F4te", "cot\u00E9");
		AssertCompare ("#inv-4", 1, "co\u0302te", "cote\u0306");
		AssertCompare ("#inv-5", 1, "co\u030Cte", "cote\u0306");

		// french
		AssertSortKey ("#fr-1", new byte [] {0xE, 0xA, 0xE, 0x7C, 0xE, 0x99, 0xE, 0x21, 1, 2, 2, 0x12, 1, 1, 1, 0}, "c\u00F4te", CompareOptions.None, french);
		AssertSortKey ("#fr-2", new byte [] {0xE, 0xA, 0xE, 0x7C, 0xE, 0x99, 0xE, 0x21, 1, 0xE, 1, 1, 1, 0}, "cot\u00E9", CompareOptions.None, french);
		AssertCompare ("#fr-3", -1, "c\u00F4te", "cot\u00E9", CompareOptions.None, french);
		// FIXME: why does .NET return 1 ?
//		AssertCompare ("#fr-4", -1, "co\u0302te", "cote\u0306", CompareOptions.None, french);
//		AssertCompare ("#fr-4", -1, "co\u030Cte", "cote\u0306", CompareOptions.None, french);
	}

	[Test]
	public void GetSortKeyThai ()
	{
		if (!doTest)
			return;

		AssertSortKey ("#i1", new byte [] {
			0x1E, 7, 0x1F, 0x28, 1, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E3A");
		AssertSortKey ("#i2", new byte [] {
			0x1E, 7, 1, 3, 1, 1, 1, 0},
			"\u0E01\u0E3B");
// FIXME: not working (table fix required)
//		AssertSortKey ("#i6", new byte [] {
//			0x1E, 7, 0xA, 0xF9, 1, 3, 1, 1, 1, 0},
//			"\u0E01\u0E3F");
		AssertSortKey ("#i7", new byte [] {
			0x1E, 7, 0x1E, 2, 1, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E40");
		AssertSortKey ("#i8", new byte [] {
			0x1E, 7, 0x1E, 3, 1, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E41");
		AssertSortKey ("#i9", new byte [] {
			0x1E, 7, 0x1E, 4, 1, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E42");
		AssertSortKey ("#i10", new byte [] {
			0x1E, 7, 0x1E, 5, 1, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E43");
		AssertSortKey ("#i11", new byte [] {
			0x1E, 7, 0x1E, 6, 1, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E44");
		AssertSortKey ("#i12", new byte [] {
			0x1E, 7, 0x1F, 0x29, 1, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E45");
		AssertSortKey ("#i13", new byte [] {
			0x1E, 7, 0x1F, 0x2A, 1, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E46");
// FIXME: not working (U+E47 table fix required)
//		AssertSortKey ("#i14", new byte [] {
//			0x1E, 7, 1, 5, 1, 1, 1, 0},
//			"\u0E01\u0E47");
		AssertSortKey ("#i15", new byte [] {
			0x1E, 7, 1, 6, 1, 1, 1, 0},
			"\u0E01\u0E48");
		AssertSortKey ("#i16", new byte [] {
			0x1E, 7, 1, 7, 1, 1, 1, 0},
			"\u0E01\u0E49");
		AssertSortKey ("#i17", new byte [] {
			0x1E, 7, 1, 8, 1, 1, 1, 0},
			"\u0E01\u0E4A");
		AssertSortKey ("#i18", new byte [] {
			0x1E, 7, 1, 9, 1, 1, 1, 0},
			"\u0E01\u0E4B");
// FIXME: not working (U+E47 table fix required)
//		AssertSortKey ("#i19", new byte [] {
//			0x1E, 7, 1, 8, 1, 1, 1, 0},
//			"\u0E01\u0E48\u0E47");
		AssertSortKey ("#i20", new byte [] {
			0x1E, 7, 0x1E, 4, 0x1E, 0xD, 1, 3, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E42\u0E02");
		AssertSortKey ("#i21", new byte [] {
			0x1E, 7, 0x1E, 0xD, 1, 3, 3, 1, 1, 1, 0},
			"\u0E01\u0E02");
	}

	[Test]
	public void GetSortKeyCzechTailoring ()
	{
		if (!doTest)
			return;

		AssertSortKey ("#i1", new byte [] {
			0xE, 0xA, 0xE, 0x2C, 1, 1, 1, 1, 0},
			"ch");
		AssertSortKey ("#cs1", new byte [] {
			0xE, 0x2E, 1, 1, 1, 1, 0},
			"ch", CompareOptions.None, czech);
		AssertSortKey ("#i2", new byte [] {
			0xE, 0x8A, 1, 0x14, 1, 1, 1, 0},
			"r\u030C");
		AssertSortKey ("#cs2", new byte [] {
			0xE, 0x8A, 1, 0x14, 1, 1, 1, 0},
			"r\u030C", CompareOptions.None, czech);
	}

	[Test]
	public void GetSortKeyHungarianTailoring ()
	{
		if (!doTest)
			return;

		AssertSortKey ("#1", new byte [] {
			0xE, 0xE, 1, 1, 0x1A, 1, 1, 0},
			"CS", CompareOptions.None, hungarian);
		AssertSortKey ("#2", new byte [] {
			0xE, 0xE, 1, 1, 0x12, 1, 1, 0},
			"Cs", CompareOptions.None, hungarian);
		AssertSortKey ("#3", new byte [] {
			0xE, 0xE, 1, 1, 1, 1, 0},
			"cs", CompareOptions.None, hungarian);
		AssertSortKey ("#4", new byte [] {
			0xE, 0x1C, 1, 1, 0x1A, 1, 1, 0},
			"DZ", CompareOptions.None, hungarian);
		AssertSortKey ("#5", new byte [] {
			0xE, 0x1C, 1, 1, 0x12, 1, 1, 0},
			"Dz", CompareOptions.None, hungarian);
		AssertSortKey ("#6", new byte [] {
			0xE, 0x1C, 1, 1, 1, 1, 0},
			"dz", CompareOptions.None, hungarian);
		AssertSortKey ("#7", new byte [] {
			0xE, 0x75, 1, 1, 0x1A, 1, 1, 0},
			"NY", CompareOptions.None, hungarian);
		AssertSortKey ("#8", new byte [] {
			0xE, 0x75, 1, 1, 0x12, 1, 1, 0},
			"Ny", CompareOptions.None, hungarian);
		AssertSortKey ("#9", new byte [] {
			0xE, 0x75, 1, 1, 1, 1, 0},
			"ny", CompareOptions.None, hungarian);
		AssertSortKey ("#10", new byte [] {
			0xE, 0xB1, 1, 1, 0x1A, 1, 1, 0},
			"ZS", CompareOptions.None, hungarian);
		AssertSortKey ("#11", new byte [] {
			0xE, 0xB1, 1, 1, 0x12, 1, 1, 0},
			"Zs", CompareOptions.None, hungarian);
		AssertSortKey ("#12", new byte [] {
			0xE, 0xB1, 1, 1, 1, 1, 0},
			"zs", CompareOptions.None, hungarian);

		// Windows seems to have bugs around repetitive characters
		// that is tailored.
//		AssertSortKey ("#x", new byte [] {
//			0xE, 0x2E, 1, 1, 1, 1, 0},
//			"CCS", CompareOptions.None, hungarian);

		// FIXME: we need to handle case insensitivity
	}

	[Test]
	public void CustomCJKTable ()
	{
		if (!doTest)
			return;

		AssertSortKey ("#1", new byte [] {
			0x9E, 9, 0x9E, 0x11, 1, 1, 1, 1, 0},
			"\u4E03\u4E09");
		AssertSortKey ("#2", new byte [] {
			0x84, 0xD3, 0x84, 0x61, 1, 1, 1, 1, 0},
			"\u4E03\u4E09", CompareOptions.None, japanese);
	}

	[Test]
	public void CultureSensitiveCompare ()
	{
		if (!doTest)
			return;

		AssertCompare ("#1", -1, "1", "2");
		AssertCompare ("#2", 1, "A", "a");
		AssertCompare ("#3", 0, "A", "a", CompareOptions.IgnoreCase);
		AssertCompare ("#4", 0, "\uFF10", "0", CompareOptions.IgnoreWidth);
		AssertCompare ("#5", 0, "\uFF21", "a", ignoreCW);
		AssertCompare ("#6", 1, "12", "1");
		AssertCompare ("#7", 0, "AE", "\u00C6");
		AssertCompare ("#8", 0, "AB\u01c0C", "A\u01c0B\u01c0C", CompareOptions.IgnoreSymbols);
		AssertCompare ("#9", 0, "A\u0304", "\u0100");
		AssertCompare ("#10", 1, "ABCABC", 5, 1, "1", 0, 1, CompareOptions.IgnoreCase, invariant);
		AssertCompare ("#11", 0, "-d:NET_2_0", 0, 1, "-", 0, 1);

		AssertCompare ("#12", 0, "ae", "\u00E6");
		AssertCompare ("#13", 0, "\u00E6", "ae");
		AssertCompare ("#14", 0, "\u00E6s", 0, 1, "ae", 0, 2);

		// target is "empty" (in culture-sensitive context).
		AssertCompare ("#17", 0, String.Empty, "\u3007");
		AssertCompare ("#18", 1, "A", "\u3007");
		AssertCompare ("#19", 1, "ABC", "\u3007");

		// shift weight comparison 
		AssertCompare ("#20", 1, "--start", "--");
		// expansion
		AssertCompare ("#21", -1, "\u00E6", "aes");
	}

	[Test]
	public void CompareSpecialWeight ()
	{
		if (!doTest)
			return;

		// Japanese (in invariant)
		AssertCompare ("#1", 1, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D");
		AssertCompare ("#2", 0, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D", CompareOptions.IgnoreWidth);
		AssertCompare ("#3", 0, "\uFF80\uFF9E\uFF72\uFF8C\uFF9E", 
			"\u30C0\u30A4\u30D6", CompareOptions.IgnoreWidth);
		AssertCompare ("#4", 1, "\u3042\u309D", "\u3042\u3042");
		AssertCompare ("#5", 0, "\u3042\u309D", "\u3042\u3042", CompareOptions.IgnoreNonSpace);
		AssertCompare ("#6", 0, "\uFF8A\uFF9E\uFF70\uFF99",
			"\u30D0\u30FC\u30EB", CompareOptions.IgnoreWidth);

		// extender in target
		AssertCompare ("#7", -1, "\u30D1\u30A2", "\u30D1\u30FC");
		AssertCompare ("#8", 0, "\u30D1\u30A2", "\u30D1\u30FC", CompareOptions.IgnoreNonSpace);
		// extender in source
		AssertCompare ("#9", 1, "\u30D1\u30FC", "\u30D1\u30A2");
		AssertCompare ("#10", 0, "\u30D1\u30FC", "\u30D1\u30A2", CompareOptions.IgnoreNonSpace);
	}

	[Test]
	public void IndexOfChar ()
	{
		if (!doTest)
			return;

		AssertIndexOf ("#1", -1, "ABC", '1');
		AssertIndexOf ("#2", 2, "ABCABC", 'c', CompareOptions.IgnoreCase);
		AssertIndexOf ("#3", 1, "ABCABC", '\uFF22', ignoreCW);
		AssertIndexOf ("#4", 4, "ABCDE", '\u0117', ignoreCN);
		AssertIndexOf ("#5", 1, "ABCABC", 'B', 1, 5, CompareOptions.IgnoreCase, invariant);
		AssertIndexOf ("#6", 4, "ABCABC", 'B', 2, 4, CompareOptions.IgnoreCase, invariant);
		AssertIndexOf ("#7", 1, "\u30D1\u30FC", '\u30A2', CompareOptions.IgnoreNonSpace);
		AssertIndexOf ("#8", 1, "UAE", '\u00C6');
		AssertIndexOf ("#8-2", 1, "AAE", '\u00C6');
		AssertIndexOf ("#9", -1, "UA", '\u00C6');
		AssertIndexOf ("#10", -1, "UE", '\u00C6');
	}

	[Test]
	[Category ("NotDotNet")]
	public void IndexOfCharMSBug ()
	{
		if (!doTest)
			return;

		AssertIndexOf ("#1", 0, "\u00E6", 'a');
	}

	[Test]
	public void LastIndexOfChar ()
	{
		if (!doTest)
			return;

		AssertLastIndexOf ("#1", -1, "ABC", '1');
		AssertLastIndexOf ("#2", 5, "ABCABC", 'c', CompareOptions.IgnoreCase);
		AssertLastIndexOf ("#3", 4, "ABCABC", '\uFF22', ignoreCW);
		AssertLastIndexOf ("#4", 4, "ABCDE", '\u0117', ignoreCN);
		AssertLastIndexOf ("#5", 1, "ABCABC", 'B', 3, 3);
		AssertLastIndexOf ("#6", 4, "ABCABC", 'B', 4, 4);
		AssertLastIndexOf ("#7", -1, "ABCABC", 'B', 5, 1);
		AssertLastIndexOf ("#8", 1, "UAE", '\u00C6');
		AssertLastIndexOf ("#8-2", 1, "UAEE", '\u00C6');
		AssertLastIndexOf ("#9", -1, "UA", '\u00C6');
		AssertLastIndexOf ("#10", -1, "UE", '\u00C6');
	}

	[Test]
	[Category ("NotDotNet")]
	public void LastIndexOfCharMSBug ()
	{
		if (!doTest)
			return;

		AssertIndexOf ("#1", 0, "\u00E6", 'a');
	}

	[Test]
	public void IsPrefix ()
	{
		if (!doTest)
			return;

		AssertIsPrefix ("#1", false, "ABC", "c", CompareOptions.IgnoreCase);
		AssertIsPrefix ("#2", false, "BC", "c", CompareOptions.IgnoreCase);
		AssertIsPrefix ("#3", true, "C", "c", CompareOptions.IgnoreCase);
		AssertIsPrefix ("#4", true, "EDCBA", "\u0117", ignoreCN);
		AssertIsPrefix ("#5", true, "ABC", "AB", CompareOptions.IgnoreCase);
		AssertIsPrefix ("#6", true, "ae", "\u00E6", CompareOptions.None);
		AssertIsPrefix ("#7", true, "\u00E6", "ae", CompareOptions.None);

		AssertIsPrefix ("#8", true, "\u00E6", "a", CompareOptions.None);
		AssertIsPrefix ("#9", true, "\u00E6s", "ae", CompareOptions.None);
		AssertIsPrefix ("#10", false, "\u00E6", "aes", CompareOptions.None);
		AssertIsPrefix ("#11", true, "--start", "--", CompareOptions.None);
		AssertIsPrefix ("#12", true, "-d:NET_1_1", "-", CompareOptions.None);
		AssertIsPrefix ("#13", false, "-d:NET_1_1", "@", CompareOptions.None);
		// U+3007 is completely ignored character.
		AssertIsPrefix ("#14", true, "\uff21\uff21", "\uff21", CompareOptions.None);
		AssertIsPrefix ("#15", true, "\uff21\uff21", "\u3007\uff21", CompareOptions.None);
		AssertIsPrefix ("#16", true, "\uff21\uff21", "\uff21\u3007", CompareOptions.None);
	}

	[Test]
	public void IsPrefixSpecialWeight ()
	{
		if (!doTest)
			return;

		// Japanese (in invariant)
		AssertIsPrefix ("#1", false, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D");
		AssertIsPrefix ("#2", true, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D", CompareOptions.IgnoreWidth);
		AssertIsPrefix ("#2-2", false, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D");
		AssertIsPrefix ("#3", true, "\uFF80\uFF9E\uFF72\uFF8C\uFF9E", 
			"\u30C0\u30A4\u30D6", CompareOptions.IgnoreWidth);
		AssertIsPrefix ("#3-2", false, "\uFF80\uFF9E\uFF72\uFF8C\uFF9E", 
			"\u30C0\u30A4\u30D6");
		AssertIsPrefix ("#4", false, "\u3042\u309D", "\u3042\u3042");
		AssertIsPrefix ("#5", true, "\u3042\u309D", "\u3042\u3042", CompareOptions.IgnoreNonSpace);
		AssertIsPrefix ("#6", true, "\uFF8A\uFF9E\uFF70\uFF99",
			"\u30D0\u30FC\u30EB", CompareOptions.IgnoreWidth);

		// extender in target
		AssertIsPrefix ("#7", false, "\u30D1\u30A2", "\u30D1\u30FC");
		AssertIsPrefix ("#8", true, "\u30D1\u30A2", "\u30D1\u30FC", CompareOptions.IgnoreNonSpace);
		// extender in source
		AssertIsPrefix ("#9", false, "\u30D1\u30FC", "\u30D1\u30A2");
		AssertIsPrefix ("#10", true, "\u30D1\u30FC", "\u30D1\u30A2", CompareOptions.IgnoreNonSpace);

		// empty suffix always matches the source.
		AssertIsPrefix ("#11", true, "", "");
		AssertIsPrefix ("#12", true, "/test.css", "");

		// bug #76243
		AssertIsPrefix ("#13", false, "\u00e4_", "a");
	}

	[Test]
	public void IsSuffix ()
	{
		if (!doTest)
			return;

		AssertIsSuffix ("#1", true, "ABC", "c", CompareOptions.IgnoreCase);
		AssertIsSuffix ("#2", true, "BC", "c", CompareOptions.IgnoreCase);
		AssertIsSuffix ("#3", false, "CBA", "c", CompareOptions.IgnoreCase);
		AssertIsSuffix ("#4", true, "ABCDE", "\u0117", CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
		AssertIsSuffix ("#5", false, "\u00E6", "a", CompareOptions.None);
		AssertIsSuffix ("#6", true, "\u00E6", "ae", CompareOptions.None);
		AssertIsSuffix ("#7", true, "ae", "\u00E6", CompareOptions.None);
		AssertIsSuffix ("#8", false, "e", "\u00E6", CompareOptions.None);
		// U+3007 is completely ignored character.
		AssertIsSuffix ("#9", true, "\uff21\uff21", "\uff21", CompareOptions.None);
		AssertIsSuffix ("#10", true, "\uff21\uff21", "\u3007\uff21", CompareOptions.None);
		AssertIsSuffix ("#11", true, "\uff21\uff21", "\uff21\u3007", CompareOptions.None);
		// extender in target
		AssertIsSuffix ("#12", false, "\u30D1\u30A2", "\u30D1\u30FC");
		AssertIsSuffix ("#13", true, "\u30D1\u30A2", "\u30D1\u30FC", CompareOptions.IgnoreNonSpace);
		// extender in source
		AssertIsSuffix ("#14", false, "\u30D1\u30FC", "\u30D1\u30A2");
		AssertIsSuffix ("#15", true, "\u30D1\u30FC", "\u30D1\u30A2", CompareOptions.IgnoreNonSpace);
		// optimization sanity check
		AssertIsSuffix ("#16", true,
			"/configuration/system.runtime.remoting",
			"system.runtime.remoting");

		// empty suffix always matches the source.
		AssertIsSuffix ("#17", true, "", "");
		AssertIsSuffix ("#18", true, "/test.css", "");
	}

	[Test]
	[Category ("NotDotNet")]
	[Category ("NotWorking")]
	public void IsSuffixMSBug ()
	{
		if (!doTest)
			return;

		AssertIsSuffix ("#1", true, "\u00E6", "e", CompareOptions.None);
	}

	[Test]
	public void IndexOfString ()
	{
		if (!doTest)
			return;

		AssertIndexOf ("#1", -1, "ABC", "1", CompareOptions.None);
		AssertIndexOf ("#2", 2, "ABCABC", "c", CompareOptions.IgnoreCase);
		AssertIndexOf ("#3", 1, "ABCABC", "\uFF22", CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth);
		AssertIndexOf ("#4", 4, "ABCDE", "\u0117", CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
		AssertIndexOf ("#5", 1, "ABCABC", "BC", CompareOptions.IgnoreCase);
		AssertIndexOf ("#6", 1, "BBCBBC", "BC", CompareOptions.IgnoreCase);
		AssertIndexOf ("#7", -1, "ABCDEF", "BCD", 0, 3, CompareOptions.IgnoreCase, invariant);
		AssertIndexOf ("#8", 0, "-ABC", "-", CompareOptions.None);
		AssertIndexOf ("#9", 0, "--ABC", "--", CompareOptions.None);
		AssertIndexOf ("#10", -1, "--ABC", "--", 1, 2, CompareOptions.None, invariant);
		AssertIndexOf ("#11", 0, "AE", "\u00C6", CompareOptions.None);
		// U+3007 is completely ignored character.
		AssertIndexOf ("#12", 0, "\uff21\uff21", "\uff21", CompareOptions.None);
		AssertIndexOf ("#13", 0, "\uff21\uff21", "\u3007\uff21", CompareOptions.None);
		AssertIndexOf ("#14", 0, "\uff21\uff21", "\uff21\u3007", CompareOptions.None);
		AssertIndexOf ("#15", 0, "\uff21\uff21", "\u3007", CompareOptions.None);
		AssertIndexOf ("#15-2", 1, "\u3007\uff21", "\uff21", CompareOptions.None);
		// target is "empty" (in culture-sensitive context).
		AssertIndexOf ("#16", -1, String.Empty, "\u3007");
		AssertIndexOf ("#17", 0, "A", "\u3007");
		AssertIndexOf ("#18", 0, "ABC", "\u3007");
	}

	[Test]
	public void IndexOfSpecialWeight ()
	{
		if (!doTest)
			return;

		// Japanese (in invariant)
		AssertIndexOf ("#1", -1, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D");
		// extender in target
		AssertIndexOf ("#1-2", -1, "\u30D1\u30A2", "\u30D1\u30FC");
		AssertIndexOf ("#1-3", 0, "\u30D1\u30A2", "\u30D1\u30FC", CompareOptions.IgnoreNonSpace);
		// extender in source
		AssertIndexOf ("#1-4", 0, "\u30D1\u30FC", "\u30D1\u30A2", CompareOptions.IgnoreNonSpace);
		AssertIndexOf ("#1-5", 1, "\u30D1\u30FC", "\u30A2", CompareOptions.IgnoreNonSpace);
		AssertIndexOf ("#2", 0, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D", CompareOptions.IgnoreWidth);
		AssertIndexOf ("#2-2", -1, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D");
		AssertIndexOf ("#3", 0, "\uFF80\uFF9E\uFF72\uFF8C\uFF9E", 
			"\u30C0\u30A4\u30D6", CompareOptions.IgnoreWidth);
		AssertIndexOf ("#4", -1, "\u3042\u309D", "\u3042\u3042");
		AssertIndexOf ("#5", 0, "\u3042\u309D", "\u3042\u3042", CompareOptions.IgnoreNonSpace);
		AssertIndexOf ("#6", 0, "\uFF8A\uFF9E\uFF70\uFF99",
			"\u30D0\u30FC\u30EB", CompareOptions.IgnoreWidth);

	}

	[Test]
	public void LastIndexOfString ()
	{
		if (!doTest)
			return;

		AssertLastIndexOf ("#1", -1, "ABC", "1", CompareOptions.None);
		AssertLastIndexOf ("#2", 5, "ABCABC", "c", CompareOptions.IgnoreCase);
		AssertLastIndexOf ("#3", 4, "ABCABC", "\uFF22", CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth);
		AssertLastIndexOf ("#4", 4, "ABCDE", "\u0117", CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
		AssertLastIndexOf ("#5", 4, "ABCABC", "BC", CompareOptions.IgnoreCase);
		AssertLastIndexOf ("#6", 4, "BBCBBC", "BC", CompareOptions.IgnoreCase);
		AssertLastIndexOf ("#7", 1, "original", "rig", CompareOptions.None);
		AssertLastIndexOf ("#8", 0, "\u00E6", "ae", CompareOptions.None);
		AssertLastIndexOf ("#9", 0, "-ABC", "-", CompareOptions.None);
		AssertLastIndexOf ("#10", 0, "--ABC", "--", CompareOptions.None);
		AssertLastIndexOf ("#11", -1, "--ABC", "--", 2, 2, CompareOptions.None, invariant);
		AssertLastIndexOf ("#12", -1, "--ABC", "--", 4, 2, CompareOptions.None, invariant);
		AssertLastIndexOf ("#13", 0, "AE", "\u00C6", CompareOptions.None);
		// U+3007 is completely ignored character.
		AssertLastIndexOf ("#14", 1, "\uff21\uff21", "\uff21", CompareOptions.None);
		AssertLastIndexOf ("#15", 1, "\uff21\uff21", "\u3007\uff21", CompareOptions.None);
		AssertLastIndexOf ("#16", 1, "\uff21\uff21", "\uff21\u3007", CompareOptions.None);
		AssertLastIndexOf ("#17", 1, "\uff21\uff21", "\u3007", CompareOptions.None);
		AssertLastIndexOf ("#18", 1, "\u3007\uff21", "\uff21", CompareOptions.None);
	}

	[Test]
	public void LastIndexOfSpecialWeight ()
	{
		if (!doTest)
			return;

		// Japanese (in invariant)
		AssertLastIndexOf ("#1", -1, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D");
		// extender in target
		AssertLastIndexOf ("#1-2", -1, "\u30D1\u30A2", "\u30D1\u30FC");
		AssertLastIndexOf ("#1-3", 0, "\u30D1\u30A2", "\u30D1\u30FC", CompareOptions.IgnoreNonSpace);
		// extender in source
		AssertLastIndexOf ("#1-4", 0, "\u30D1\u30FC", "\u30D1\u30A2", CompareOptions.IgnoreNonSpace);
		// FIXME: not working (extender support is not complete. 
		// Currently private IsPrefix() cannot handle heading
		// extenders to consume previous primary char.)
//		AssertLastIndexOf ("#1-5", 1, "\u30D1\u30FC", "\u30A2", CompareOptions.IgnoreNonSpace);
		// this shows that Windows accesses beyond the length and
		// acquires the corresponding character to expand.
//		AssertLastIndexOf ("#1-6", 1, "\u30D1\u30FC", "\u30A2", 1, 1, CompareOptions.IgnoreNonSpace, invariant);
		AssertLastIndexOf ("#2", 0, "\u30D1\u30FC\u30B9", "\uFF8A\uFF9F\uFF70\uFF7D", CompareOptions.IgnoreWidth);
		AssertLastIndexOf ("#3", 0, "\uFF80\uFF9E\uFF72\uFF8C\uFF9E", 
			"\u30C0\u30A4\u30D6", CompareOptions.IgnoreWidth);
		AssertLastIndexOf ("#4", -1, "\u3042\u309D", "\u3042\u3042");
		AssertLastIndexOf ("#5", 0, "\u3042\u309D", "\u3042\u3042", CompareOptions.IgnoreNonSpace);
		AssertLastIndexOf ("#6", 0, "\uFF8A\uFF9E\uFF70\uFF99",
			"\u30D0\u30FC\u30EB", CompareOptions.IgnoreWidth);
	}
}

}
