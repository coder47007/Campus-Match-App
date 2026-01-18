// TypeScript types matching CampusMatch C# API DTOs

// Auth DTOs
export interface RegisterRequest {
    email: string;
    password: string;
    name: string;
    phoneNumber: string;
    instagramHandle?: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface AuthResponse {
    token: string;
    refreshToken?: string;
    student: StudentDto;
    isAdmin?: boolean;
}

export interface RefreshTokenRequest {
    refreshToken: string;
}

export interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
}

export interface DeleteAccountRequest {
    password: string;
}

export interface ForgotPasswordRequest {
    email: string;
}

export interface ResetPasswordRequest {
    email: string;
    token: string;
    newPassword: string;
}

// Student Profile DTOs
export interface StudentDto {
    id: number;
    email: string;
    name: string;
    age?: number;
    major?: string;
    year?: string;
    bio?: string;
    photoUrl?: string;
    university?: string;
    gender?: string;
    preferredGender?: string;
    phoneNumber?: string;
    instagramHandle?: string;
    latitude?: number;
    longitude?: number;
    interests?: InterestDto[];
    photos?: PhotoDto[];
    prompts?: StudentPromptDto[];
}

export interface ProfileUpdateRequest {
    name: string;
    age?: number;
    major?: string;
    year?: string;
    bio?: string;
    photoUrl?: string;
    university?: string;
    gender?: string;
    preferredGender?: string;
    phoneNumber?: string;
    instagramHandle?: string;
    latitude?: number;
    longitude?: number;
}

// Photo DTOs
export interface PhotoDto {
    id: number;
    url: string;
    isPrimary: boolean;
    displayOrder: number;
}

// Interest DTOs
export interface InterestDto {
    id: number;
    name: string;
    emoji: string;
    category: string;
}

export interface UpdateInterestsRequest {
    interestIds: number[];
}

// Prompt DTOs
export interface PromptDto {
    id: number;
    question: string;
    category: string;
}

export interface StudentPromptDto {
    id: number;
    promptId: number;
    question: string;
    answer: string;
    displayOrder: number;
}

export interface UpdatePromptRequest {
    promptId: number;
    answer: string;
}

export interface UpdatePromptsRequest {
    prompts: UpdatePromptRequest[];
}

// Swipe DTOs
export interface SwipeRequest {
    swipedId: number;
    isLike: boolean;
    isSuperLike?: boolean;
}

export interface SwipeResponse {
    isMatch: boolean;
    match?: MatchDto;
}

export interface UndoSwipeResponse {
    success: boolean;
    message?: string;
}

// Likes DTOs
export interface LikePreviewDto {
    id: number;
    blurredPhotoUrl?: string;
    firstLetter: string;
    isSuperLike: boolean;
    likedAt: string;
}

// Match DTOs
export interface MatchDto {
    id: number;
    otherStudentId: number;
    otherStudentName: string;
    otherStudentPhotoUrl?: string;
    otherStudentMajor?: string;
    matchedAt: string;
    isActive?: boolean;
    lastMessage?: string;
}

// Message DTOs
export interface MessageDto {
    id: number;
    senderId: number;
    senderName: string;
    content: string;
    sentAt: string;
    isRead: boolean;
    deliveredAt?: string;
    readAt?: string;
}

export interface SendMessageRequest {
    matchId: number;
    content: string;
}

// Settings DTOs
export interface SettingsDto {
    minAgePreference: number;
    maxAgePreference: number;
    maxDistancePreference: number;
    showOnlineStatus: boolean;
    notifyOnMatch: boolean;
    notifyOnMessage: boolean;
    notifyOnSuperLike: boolean;
    isProfileHidden: boolean;
}

export interface UpdateSettingsRequest {
    minAgePreference?: number;
    maxAgePreference?: number;
    maxDistancePreference?: number;
    showOnlineStatus?: boolean;
    notifyOnMatch?: boolean;
    notifyOnMessage?: boolean;
    notifyOnSuperLike?: boolean;
    isProfileHidden?: boolean;
}

// Report & Block DTOs
export interface ReportRequest {
    reportedId: number;
    reason: string;
    details?: string;
    source?: 'chat' | 'discover';
}

export interface BlockRequest {
    blockedId: number;
}

export interface BlockedUserDto {
    id: number;
    name: string;
    photoUrl?: string;
    blockedAt: string;
}

// Rewinds
export interface RewindsRemainingResponse {
    remaining: number;
    resetsAt: string;
}

// Likes count
export interface LikesCountResponse {
    total: number;
    superLikes: number;
    hasNew: boolean;
}
