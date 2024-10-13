export const NotFound = () => {
  return (
    <div id="error-page" className={"flex flex-col justify-center items-center h-screen"}>
      <h1 className={"text-3xl mb-2 font-medium"}>404: Not Found</h1>
      <a href={"/inkless"} className={"underline hover:font-medium"}>На главную</a>
    </div>
  );
}