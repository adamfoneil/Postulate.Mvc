namespace Postulate.Mvc
{
	public class ApiResult<T>
	{
		private readonly T _data;

		public ApiResult(T data)
		{
			_data = data;
		}

		public bool IsSuccessful { get; set; }
		public string Message { get; set; }
		public T Data { get { return _data; } }
	}
}