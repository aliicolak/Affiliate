using System;
using System.Collections.Generic;

namespace Application.Features.Social;

public record CollectionDto(long Id, string Name, string? Description, bool IsPublic, bool IsDefault, int ItemCount, DateTime CreatedUtc);

public record CollectionDetailDto(long Id, string Name, string? Description, bool IsPublic, bool IsDefault, List<CollectionItemDto> Items);

public record CollectionItemDto(long Id, string EntityType, long EntityId, object? EntityData, DateTime AddedUtc);
