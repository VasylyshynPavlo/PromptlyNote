using PromptlyNote.Core.Utils;

namespace PromptlyNote.Core.Constants
{
    public static class ExceptionMessages
    {
        /// <summary>
        /// Returns a message indicating that the provided ID has an invalid format.
        /// </summary>
        /// <param name="name">The name of the entity or field whose ID is invalid (e.g. "category", "user").</param>
        /// <returns>Example: "Invalid {name} ID format."</returns>
        public static string InvalidIdFormat(string name) => $"Invalid {name} ID format.";

        /// <summary>
        /// Returns a message indicating that the specified entity was not found.
        /// </summary>
        /// <param name="entity">The name of the entity that was not found (e.g. "Category", "User").</param>
        /// <returns>Example: "{Entity} not found."</returns>
        public static string NotFound(string entity) => $"{entity.CapitalizeFirst()} not found.";

        /// <summary>
        /// Returns a message indicating that an entity with the same name already exists.
        /// </summary>
        /// <param name="entity">The name of the entity type that already exists (e.g. "category", "task list").</param>
        /// <returns>Example: "A {entity} with the same {fieldName} already exists."</returns>
        public static string ConflictFieldsName(string entity, string fieldName) => $"A {entity} with the same {fieldName} already exists.";

        /// <summary>
        /// Returns a message indicating that the user does not have permission to perform the specified action.
        /// </summary>
        /// <param name="action">The action being attempted (e.g. "delete", "update").</param>
        /// <param name="entity">The entity the action is being performed on (e.g. "category", "task").</param>
        /// <returns>Example: "You do not have permission to {action} this {entity}."</returns>
        public static string NoPermission(string action, string entity) => $"You do not have permission to {action} this {entity}.";

        /// <summary>
        /// Returns a message indicating that the user is not the owner of the specified entity.
        /// </summary>
        /// <param name="entity">The name of the entity that the user is not the owner of (e.g. "category", "task").</param>
        /// <returns>Example: "You are not the owner of this {entity}."</returns>
        public static string NotOwner(string entity) => $"You are not the owner of this {entity}.";

        /// <summary>
        /// Returns a message indicating that the provided sort option for the specified entity is invalid.
        /// </summary>
        /// <param name="entity">The name of the entity for which the sort option is invalid.</param>
        /// <returns>Example: "Invalid sort option for {entity}."</returns>
        public static string InvalidSortBy(string entity) => $"Invalid sort option for {entity}.";

        /// <summary>
        /// Returns a message indicating that the specified field is required.
        /// </summary>
        /// <param name="fieldName">The name of the field that is required.</param>
        /// <returns>Example: "{FieldName} is required."</returns>
        public static string FieldIsRequired(string fieldName) => $"{fieldName.CapitalizeFirst()} is required.";
    }
}