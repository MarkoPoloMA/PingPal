//using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
//using System.ComponentModel.DataAnnotations;
//using System.Reflection;
//using System.Resources;

//namespace Messenger.Helpers;


//public static class ComponentModelResourceManagerHelper
//{
//    public static void OverrideResourceManager()
//    {
//        EnsureAssemblyIsLoaded();

//        var resourceManagerFieldInfo = GetResourceManagerFieldInfo();
//        var resourceManager = GetNewResourceManager();
//        resourceManagerFieldInfo.SetValue(null, resourceManager);
//    }

//    private static void EnsureAssemblyIsLoaded()
//    {
//        // ReSharper disable UseDiscardAssignment
//        var _ = typeof(RequiredAttribute);
//        // ReSharper restore UseDiscardAssignment
//    }

//    private static FieldInfo GetResourceManagerFieldInfo()
//    {
//        var srAssembly = AppDomain.CurrentDomain
//            .GetAssemblies()
//            .First(assembly => assembly.FullName
//                ?.StartsWith("System.ComponentModel.Annotations,", StringComparison.Ordinal) == true);

//        var srType = srAssembly.GetType("System.SR")
//            ?? throw new NullReferenceException("Type System.SR is missing");

//        return srType.GetField("s_resourceManager", BindingFlags.Static | BindingFlags.NonPublic)
//            ?? throw new NullReferenceException("Field s_resourceManager is missing");
//    }

//    private static ResourceManager GetNewResourceManager()
//    {
//        return new ResourceManager($"{typeof(ComponentModelResource).FullName}", typeof(ComponentModelResource).Assembly);
//    }

//    public static void SetAccessorMessages(DefaultModelBindingMessageProvider provider)
//    {
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetValueIsInvalidAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetValueMustBeANumberAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetMissingBindRequiredValueAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetAttemptedValueIsInvalidAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetMissingKeyOrValueAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetUnknownValueIsInvalidAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetValueMustNotBeNullAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetMissingRequestBodyRequiredValueAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetNonPropertyAttemptedValueIsInvalidAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetNonPropertyUnknownValueIsInvalidAccessor));
//        ConfigureSetAccessorMethod(provider, nameof(provider.SetNonPropertyValueMustBeANumberAccessor));
//    }

//    private static void ConfigureSetAccessorMethod(
//        DefaultModelBindingMessageProvider provider,
//        string setAccessorMethodName)
//    {
//        var resourceName = setAccessorMethodName[3..];
//        var format = ComponentModelResource.ResourceManager.GetString(resourceName)
//            ?? throw new NullReferenceException($"Resource {resourceName} is missing");

//        var method = provider.GetType()
//                .GetMethod(setAccessorMethodName, BindingFlags.Instance | BindingFlags.Public)
//            ?? throw new NullReferenceException($"{setAccessorMethodName} is missing");

//        var funcParameterType = method.GetParameters().First().ParameterType;

//        if (funcParameterType == typeof(Func<string>))
//            method.Invoke(provider, [() => format]);
//        else if (funcParameterType == typeof(Func<string, string>))
//            method.Invoke(provider, [(string x) => string.Format(format, x)]);
//        else if (funcParameterType == typeof(Func<string, string, string>))
//            method.Invoke(provider, [(string x, string y) => string.Format(format, x, y)]);
//        else
//            throw new ArgumentOutOfRangeException(nameof(funcParameterType));
//    }
//}
