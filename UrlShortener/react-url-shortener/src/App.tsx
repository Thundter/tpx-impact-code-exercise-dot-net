import React, { useState } from 'react'
import { UrlList } from './components/UrlList'
import './index'
import { Shortener } from './components/Shortener'

const App = () => {
    const [refreshKey, setRefreshKey] = useState(0)

    const triggerRefresh = () => { 
        console.log("App:triggerRefresh")
        setRefreshKey(prev => prev + 1) 
    }

    return (
        <div>
            <h1>Url Shortener</h1>
            <Shortener onRefresh={triggerRefresh} />
            <h3>Shortened Urls</h3>
            <UrlList refreshCount={refreshKey} />
        </div>
    )
}

export default App