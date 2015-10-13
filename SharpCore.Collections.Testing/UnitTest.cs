using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Collections.Generic;

namespace SharpCore.Collections.Testing
{
	/// <summary>
	/// Verifies the functionality found in SharpCore.Collections.
	/// </summary>
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public void SynchronizedDictionaryUnitTest()
		{
			SynchronizedDictionary<int, string> dictionary = new SynchronizedDictionary<int, string>();

			dictionary.Add(1, "one");
			dictionary.Add(2, "two");
			Assert.IsTrue(dictionary.Count == 2);

			foreach (KeyValuePair<int, string> keyValuePair in dictionary)
			{
				Assert.IsTrue(dictionary.ContainsKey(keyValuePair.Key));
				Assert.IsTrue(dictionary.ContainsValue(keyValuePair.Value));
			}

			dictionary.Remove(1);
			Assert.IsTrue(dictionary.Count == 1);

			dictionary.Clear();
			Assert.IsTrue(dictionary.Count == 0);
		}

		[TestMethod]
		public void SynchronizedLinkedListUnitTest()
		{
			SynchronizedLinkedList<string> linkedList = new SynchronizedLinkedList<string>();

			linkedList.AddFirst("one");
			linkedList.AddLast("two");
			linkedList.AddAfter(linkedList.Find("two"), "three");
			linkedList.AddLast("four");
			Assert.IsTrue(linkedList.Count == 4);

			foreach (string name in linkedList)
			{
				Assert.IsTrue(linkedList.Contains(name));
			}

			linkedList.RemoveFirst();
			Assert.IsFalse(linkedList.Contains("one"));

			linkedList.RemoveLast();
			Assert.IsFalse(linkedList.Contains("four"));

			linkedList.Remove("two");
			Assert.IsFalse(linkedList.Contains("two"));

			linkedList.Clear();
			Assert.IsTrue(linkedList.Count == 0);
		}

		[TestMethod]
		public void SynchronizedListUnitTest()
		{
			SynchronizedList<string> list = new SynchronizedList<string>();

			list.Add("one");
			list.Add("two");
			Assert.IsTrue(list.Count == 2);

			foreach (string name in list)
			{
				Assert.IsTrue(list.Contains(name));
			}

			list.Remove("one");
			Assert.IsTrue(list.Count == 1);

			list.Insert(0, "one");
			Assert.IsTrue(list.Count == 2);
			Assert.IsTrue(list.Contains("one"));
			Assert.IsTrue(list[0] == "one");

			list.RemoveAt(0);
			Assert.IsFalse(list.Contains("one"));

			list.Clear();
			Assert.IsTrue(list.Count == 0);
		}

		[TestMethod]
		public void TestSynchronizedQueue()
		{
			SynchronizedQueue<string> queue = new SynchronizedQueue<string>();

			queue.Enqueue("one");
			queue.Enqueue("two");
			Assert.IsTrue(queue.Count == 2);

			foreach (string name in queue)
			{
				Assert.IsTrue(queue.Contains(name));
			}

			if (queue.Peek() == "one")
			{
				queue.Dequeue();
				queue.Enqueue("one");
			}
			
			Assert.IsTrue(queue.Count == 2);
			Assert.IsTrue(queue.Peek() == "two");

			queue.Clear();
			Assert.IsTrue(queue.Count == 0);
		}

		[TestMethod]
		public void SynchronizedSortedDictionaryUnitTest()
		{
			SynchronizedSortedDictionary<int, string> sortedDictionary = new SynchronizedSortedDictionary<int, string>();

			sortedDictionary.Add(1, "one");
			sortedDictionary.Add(2, "two");
			Assert.IsTrue(sortedDictionary.Count == 2);

			foreach (int key in sortedDictionary.Keys)
			{
				Assert.IsTrue(sortedDictionary.Keys.Contains(key));
				Assert.IsTrue(sortedDictionary.ContainsKey(key));
			}

			foreach (string value in sortedDictionary.Values)
			{
				Assert.IsTrue(sortedDictionary.Values.Contains(value));
				Assert.IsTrue(sortedDictionary.ContainsValue(value));
			}

			sortedDictionary.Remove(1);
			Assert.IsTrue(sortedDictionary.Count == 1);

			sortedDictionary.Clear();
			Assert.IsTrue(sortedDictionary.Count == 0);
		}

		[TestMethod]
		public void SynchronizedSortedListUnitTest()
		{
			SynchronizedSortedList<int, string> sortedList = new SynchronizedSortedList<int, string>();

			sortedList.Add(1, "one");
			sortedList.Add(2, "two");
			Assert.IsTrue(sortedList.Count == 2);

			foreach (int key in sortedList.Keys)
			{
				Assert.IsTrue(sortedList.Keys.Contains(key));
				Assert.IsTrue(sortedList.ContainsKey(key));
			}

			foreach (string value in sortedList.Values)
			{
				Assert.IsTrue(sortedList.Values.Contains(value));
				Assert.IsTrue(sortedList.ContainsValue(value));
			}

			sortedList.Remove(1);
			Assert.IsTrue(sortedList.Count == 1);

			sortedList.Clear();
			Assert.IsTrue(sortedList.Count == 0);
		}

		[TestMethod]
		public void TestSynchronizedStack()
		{
			SynchronizedStack<string> stack = new SynchronizedStack<string>();

			stack.Push("one");
			stack.Push("two");
			Assert.IsTrue(stack.Count == 2);

			foreach (string name in stack)
			{
				Assert.IsTrue(stack.Contains(name));
			}

			string peeked = stack.Peek();
			Assert.AreEqual(peeked, stack.Pop());
			Assert.IsTrue(stack.Count == 1);

			stack.Clear();
			Assert.IsTrue(stack.Count == 0);
		}
	}
}
