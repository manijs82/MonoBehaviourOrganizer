using System;

[AttributeUsage(AttributeTargets.Method)]
public class LevelerMethodAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class LevelerPropertyAttribute : Attribute { }