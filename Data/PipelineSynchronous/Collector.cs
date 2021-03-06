﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Data.PipelineSynchronous
{
	public class Collector<T> : IHandleResult<T>
	{
		private readonly List<T> _results = new List<T>();
		private Exception _error;

		public List<T> Results
		{
			get
			{
				if (_error != null)
				{
					throw new AggregateException(_error);
				}
				return _results;
			}
		}

		public void HandleData(T data)
		{
			Results.Add(data);
		}

		public void HandleError(Exception error)
		{
			_error = error;
		}

		public void HandleDone()
		{
		}

		public void Describe(TextWriter output, int myLevel)
		{
			new NodeDisplay("Collector").WriteTo(output, myLevel);
		}
	}
}
