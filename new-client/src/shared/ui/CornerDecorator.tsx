export const CornerDecorator = () => {
  return (
    <>
      <div className="absolute h-2 border-black/50 border-x top-0 left-0 right-0"></div>
      <div className="absolute w-2 border-black/50 border-y top-0 bottom-0 left-0"></div>
      <div className="absolute h-2 border-black/50 border-x bottom-0 left-0 right-0"></div>
      <div className="absolute w-2 border-black/50 border-y top-0 bottom-0 right-0"></div>
    </>
  )
}
