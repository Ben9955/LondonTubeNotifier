type SectionProps = {
  title: string;
  tag?: string;
  description?: string;
  children?: React.ReactNode;
};

function Section({ title, tag, description, children }: SectionProps) {
  return (
    <section className="py-12 md:py-16 px-5">
      <div className="text-center px-5">
        {tag && <h4 className="ont-semibold text-lg md:text-xl">{tag}</h4>}
        <h2 className="text-2xl md:text-3xl font-semibold my-2">{title}</h2>
        {description && <p className="text-base md:text-lg">{description}</p>}
      </div>
      {children}
    </section>
  );
}

export default Section;
