namespace Postulate.Mvc
{
	public class ApiResult
	{
		public bool IsSuccessful { get; set; }
		public string Message { get; set; }
	}

	public class ApiResult<T> : ApiResult
	{
		private readonly T _data;

		public ApiResult(T data)
		{
			_data = data;
		}

		public T Data { get { return _data; } }
	}
}