type ButtonProps = {
  children: React.ReactNode;
  className?: string;
};

const Button = ({ children, className }: ButtonProps) => {
  return (
    <button
      className={`px-5 py-2 rounded-md bg-blue-600 text-amber-200 hover:bg-blue-700 transition ${className}`}
    >
      {children}
    </button>
  );
};

export default Button;
