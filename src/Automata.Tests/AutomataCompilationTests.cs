﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Automata;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Automata.Tests
{
    [TestClass]
    public class AutomataCompilationTests
    {
        //[TestMethod]
        //public void CompileTest()
        //{
        //    var solver = new CharSetSolver();
        //    Tuple<int, int> foo = new Tuple<int, int>(1, 1);
        //    string regex = @"ab|ac";
        //    IEnumerable<int> bar = null;
        //    var fa = solver.Convert(regex);
        //    var fadet = fa.Determinize().Minimize();
        //    //fadet.ShowGraph("fadet");
        //    var cs = fadet.Compile();
        //    var aut = cs.Automaton;
        //    Assert.AreEqual<int>(aut.Transition(0, 'd'), 0);
        //    Assert.AreEqual<int>(aut.Transition(0, 'c', 'a'), 1);
        //    Assert.AreEqual<int>(aut.Transition(1, 'b', 'f', 'o', 'o'), 2);
        //    Assert.IsTrue(aut.IsFinalState(2));
        //    Assert.IsTrue(aut.IsSinkState(2));
        //    Assert.IsFalse(aut.IsSinkState(1));
        //    Assert.IsFalse(aut.IsFinalState(0));
        //}

        [TestMethod]
        public void CompileEvilRegex()
        {
            var regex = @"^(([a-z])+.)+[A-Z]([a-z])+$";
            Regex EvilRegex = new Regex(regex, RegexOptions.Compiled | (RegexOptions.Singleline));
            string a = "aaaaaaaaaaaaaaaaaaaa";
            //takes time exponential in the length of a
            int t = 0;
            for (int i = 0; i < 15; i++)
            {
                t = System.Environment.TickCount;
                EvilRegex.IsMatch(a);
                t = System.Environment.TickCount - t;
                a += "a";
            }
            Assert.IsTrue(t > 1000);
            var solver = new CharSetSolver();
            var fa = solver.Convert(regex);
            var fadet = fa.Determinize().Minimize();
            var cs = fadet.Compile();
            //fadet.ShowGraph("EvilRegex");
        }

        [TestMethod]
        public void GenerateMatchesTest1()
        {
            var r = new Regex("(fox*o|bar|ba+zz)");
            //r.Display("DFA",true);
            var aut = r.Compile();
            var str = "foofoxxxobaaaazzbar";
            var matches = aut.Matches(str);
            ValidateAgainstDotNet(r, str, matches);
        }

        private static void ValidateAgainstDotNet(Regex r, string str, Tuple<int, int>[] matches)
        {
            var regex_matches = r.Matches(str);
            Assert.AreEqual<int>(matches.Length, regex_matches.Count);
            foreach (Match match in regex_matches)
            {
                Assert.IsTrue(Array.Exists(matches, m => m.Item1 == match.Index && m.Item2 == match.Length));
            }
        }

        [TestMethod]
        public void GenerateMatchesTest2()
        {
            string pattern = @"(abc|aBe)";
            string str = "aaaabcccaBe";
            var r = new Regex(pattern);
            var aut = r.Compile();
            var matches = aut.Matches(str);
            ValidateAgainstDotNet(r, str, matches);
        }

        [TestMethod]
        public void GenerateMatchesTest3()
        {
            string pattern = @"(?i:p(a|@)(s|\$)+w(o|0)rd|user@123|abc@123)";
            string input = "xxPassxPasssWordisnotmyppp@sSw0RddddforUser@12333";
            var r = new Regex(pattern);
            //r.Display();
            var aut = r.Compile();
            var matches = aut.Matches(input);
            ValidateAgainstDotNet(r, input, matches);
        }
    }
}
