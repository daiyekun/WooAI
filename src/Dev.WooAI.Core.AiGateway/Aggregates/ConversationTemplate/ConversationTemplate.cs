using Dev.WooAI.SharedKernel.Domain;

namespace Dev.WooAI.Core.AiGateway.Aggregates.ConversationTemplate;

public class ConversationTemplate : IAggregateRoot
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string SystemPrompt { get; set; }

    public Guid ModelId { get; set; }

    public TemplateSpecification Specification { get; set; }
    
    public bool IsEnabled { get; set; }
    
    protected ConversationTemplate() { }
    
    public ConversationTemplate(
        string name,
        string description,
        string systemPrompt,
          Guid modelId,
        TemplateSpecification specification)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        SystemPrompt = systemPrompt;
        Specification = specification;
        IsEnabled = true;
        ModelId= modelId;
    }


    public void UpdateSpecification(TemplateSpecification spec)
    {
        Specification = spec;
    }
}