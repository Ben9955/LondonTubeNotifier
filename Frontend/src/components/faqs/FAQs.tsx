import { faqs } from "../../data/faqs";
import FAQItem from "./FAQItem";

function FAQs() {
  return (
    <div className="mt-5 space-y-4">
      {faqs.map((f) => (
        <FAQItem key={f.question} {...f} />
      ))}
    </div>
  );
}

export default FAQs;
