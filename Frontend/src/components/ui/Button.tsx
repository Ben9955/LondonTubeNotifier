import { buttonBase } from "./buttonVariants";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  children: React.ReactNode;
};

const Button = ({ children, className = "", ...props }: ButtonProps) => {
  return (
    <button
      {...props}
      className={`${buttonBase}
        ${
          props.disabled ? "opacity-50 cursor-not-allowed" : "hover:bg-blue-700"
        } 
        ${className}`}
    >
      {children}
    </button>
  );
};

export default Button;
