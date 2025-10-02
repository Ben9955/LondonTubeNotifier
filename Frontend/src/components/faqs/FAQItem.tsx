type FAQItemProps = {
  question: string;
  answer: string;
};

const FAQItem = ({ question, answer }: FAQItemProps) => {
  return (
    <div className="bg-gray-100 rounded-md p-4">
      <p className="font-semibold">{question}</p>
      <p className="mt-1 text-gray-700">{answer}</p>
    </div>
  );
};

export default FAQItem;
