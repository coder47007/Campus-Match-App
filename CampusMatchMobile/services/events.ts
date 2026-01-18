// Events API Service
import api from './api';

export interface EventDto {
    id: number;
    title: string;
    description: string;
    startTime: string;
    endTime?: string;
    location: string;
    imageUrl?: string;
    category: string;
    creatorId: number;
    creatorName: string;
    creatorPhotoUrl?: string;
    maxAttendees: number;
    attendeeCount: number;
    isAttending: boolean;
    isCreator: boolean;
    createdAt: string;
}

export interface CreateEventRequest {
    title: string;
    description?: string;
    startTime: string;
    endTime?: string;
    location?: string;
    imageUrl?: string;
    category?: string;
    maxAttendees?: number;
}

export interface UpdateEventRequest {
    title?: string;
    description?: string;
    startTime?: string;
    endTime?: string;
    location?: string;
    imageUrl?: string;
    category?: string;
    maxAttendees?: number;
}

export interface AttendeeDto {
    studentId: number;
    name: string;
    photoUrl?: string;
    joinedAt: string;
}

export interface GetEventsParams {
    category?: string;
    startAfter?: string;
    page?: number;
    pageSize?: number;
}

export const eventsApi = {
    // Get all events
    getEvents: async (params?: GetEventsParams): Promise<EventDto[]> => {
        const queryParams = new URLSearchParams();
        if (params?.category) queryParams.append('category', params.category);
        if (params?.startAfter) queryParams.append('startAfter', params.startAfter);
        if (params?.page) queryParams.append('page', params.page.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());

        const query = queryParams.toString();
        const response = await api.get<EventDto[]>(`/events${query ? `?${query}` : ''}`);
        return response.data;
    },

    // Get single event
    getEvent: async (id: number): Promise<EventDto> => {
        const response = await api.get<EventDto>(`/events/${id}`);
        return response.data;
    },

    // Get my events (created by me)
    getMyEvents: async (): Promise<EventDto[]> => {
        const response = await api.get<EventDto[]>('/events/my-events');
        return response.data;
    },

    // Get events I'm attending
    getAttendingEvents: async (): Promise<EventDto[]> => {
        const response = await api.get<EventDto[]>('/events/attending');
        return response.data;
    },

    // Create event
    createEvent: async (data: CreateEventRequest): Promise<EventDto> => {
        const response = await api.post<EventDto>('/events', data);
        return response.data;
    },

    // Update event
    updateEvent: async (id: number, data: UpdateEventRequest): Promise<EventDto> => {
        const response = await api.put<EventDto>(`/events/${id}`, data);
        return response.data;
    },

    // Delete/cancel event
    deleteEvent: async (id: number): Promise<void> => {
        await api.delete(`/events/${id}`);
    },

    // Join event
    joinEvent: async (id: number): Promise<void> => {
        await api.post(`/events/${id}/join`);
    },

    // Leave event
    leaveEvent: async (id: number): Promise<void> => {
        await api.post(`/events/${id}/leave`);
    },

    // Get event attendees
    getAttendees: async (id: number): Promise<AttendeeDto[]> => {
        const response = await api.get<AttendeeDto[]>(`/events/${id}/attendees`);
        return response.data;
    },

    // Get categories
    getCategories: async (): Promise<string[]> => {
        const response = await api.get<string[]>('/events/categories');
        return response.data;
    },
};

export default eventsApi;
