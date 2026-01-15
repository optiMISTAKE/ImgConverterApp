import apiClient from "./apiClient";
import { UserImage } from "../../Models/UserImage";

export interface ConvertResponse {
    id: string;
    originalName: string;
    storedName: string;
    downloadUrl: string;
}

const ImageService = {
    // Upload & Convert
    convertImage: async (file: File) => {
        const formData = new FormData();
        formData.append('file', file);

        // Content-Type is optional here; axios sets boundary automatically for FormData
        const response = await apiClient.post<ConvertResponse>('/Image/convert', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });
        return response.data;
    },

    // Get History
    getHistory: async () => {
        const response = await apiClient.get<UserImage[]>('/Image/history');
        return response.data;
    },

    // Download File (Blob handling)
    downloadImage: async (id: string, fileName: string) => {
        const response = await apiClient.get(`/Image/download/${id}`, {
            responseType: 'blob' // Important!
        });

        // Create a temporary link to trigger download in browser
        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', fileName); 
        document.body.appendChild(link);
        link.click();
        link.parentNode?.removeChild(link);
    },

    // Delete Selected
    deleteImages: async (ids: string[]) => {
        await apiClient.delete('/Image/delete-multiple', { data: ids });
    },

    // Delete All
    deleteAll: async () => {
        await apiClient.delete('/Image/delete-all');
    }
};

export default ImageService;
